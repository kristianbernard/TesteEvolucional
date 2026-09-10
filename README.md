# Teste Escola – API de Matrículas

API de controle de matrículas de uma escola, na stack do teste:
**.NET Framework 4.8 + ASP.NET Web API + Dapper (SQL na mão) + SQL Server**.
Sem .NET Core, sem Entity Framework.

---

## Sumário

- [Stack e requisitos atendidos](#stack-e-requisitos-atendidos)
- [Estrutura da solução](#estrutura-da-solução)
- [Pré-requisitos](#pré-requisitos)
- [Passo a passo para rodar](#passo-a-passo-para-rodar)
  - [1. Criar o banco](#1-criar-o-banco)
  - [2. Connection string](#2-connection-string)
  - [3. Subir a API](#3-subir-a-api)
  - [4. Swagger](#4-swagger)
  - [5. Abrir a tela de alunos](#5-abrir-a-tela-de-alunos)
- [Endpoints](#endpoints)
- [Regra de matrícula e transação](#regra-de-matrícula-e-transação)
- [Decisões de arquitetura](#decisões-de-arquitetura)
- [Alterações no script do banco](#alterações-no-script-do-banco)
- [Itens bônus](#itens-bônus)
- [O que ficou de fora e como eu faria](#o-que-ficou-de-fora-e-como-eu-faria)

---

## Stack e requisitos atendidos

| Requisito | Como foi atendido |
|---|---|
| .NET Framework 4.8, ASP.NET Web API (não Core) | `TargetFrameworkVersion v4.8`, `System.Web.Http` (Web API 5.2.9), hospedagem `System.Web` / IIS Express |
| Dapper + SQL escrito na mão | Todos os repositórios em `TesteEscola.Infrastructure`; nenhum ORM que gere SQL |
| SQL Server | SQL Server Express por padrão (também roda em LocalDB) |
| Status HTTP corretos (200/201/400/404/409, sem 500 para validação) | `DomainExceptionFilterAttribute` + `ValidateModelStateFilterAttribute` |
| Projeto em camadas, sem regra em controller | Domain / Services / Infrastructure / Api |
| Paginação com total | `PagedResult<T>` no corpo (`total`, `totalPaginas`) + header `X-Total-Count` |
| Exclusão lógica | `DELETE` seta `Ativo = 0`; listagem esconde inativos por padrão |
| Transação na matrícula | `insert` + decremento de `VagasDisponiveis` na mesma transação com `UPDLOCK/HOLDLOCK` |
| Relatório em SQL | `LEFT JOIN` + `GROUP BY`, sem montar em memória |

Extras: **Swagger** (Swashbuckle) em `/swagger` e **tela HTML + jQuery**
consumindo a listagem de alunos.

---

## Estrutura da solução

```
TesteEscola.sln
└─ src/
   ├─ TesteEscola.Domain          # entidades, DTOs, interfaces, exceções (sem dependências)
   ├─ TesteEscola.Services        # regras de negócio (depende só do Domain)
   ├─ TesteEscola.Infrastructure  # repositórios Dapper, SqlConnectionFactory
   └─ TesteEscola.Api             # Web API: controllers, DI (Autofac), filtros, tela
```

Fluxo de uma requisição:

```
Controller  ->  Service (regras)  ->  Repository (Dapper/SQL)  ->  SQL Server
     ^                 |
     |                 +-- lança NotFound / BusinessRule / ValidationException
     +-- ExceptionFilter traduz a exceção em 404 / 409 / 400
```

Referências entre projetos: `Api -> Services, Infrastructure, Domain`;
`Services -> Domain`; `Infrastructure -> Domain`. O Domain não referencia ninguém.

---

## Pré-requisitos

- Windows com **.NET Framework 4.8** (Developer Pack para compilar).
- **Visual Studio 2019/2022** (com a carga "ASP.NET e desenvolvimento web")
  **ou** o **MSBuild** do Build Tools + **IIS Express**.
- **SQL Server Express** (a config padrão usa `.\SQLEXPRESS` com login `sa`)
  **ou** **SQL Server LocalDB** (vem com o VS/SSMS).
- `sqlcmd` ou SQL Server Management Studio para executar o script.

---

## Passo a passo para rodar

### 1. Criar o banco

O script está em [`script-banco.sql`](script-banco.sql) (na raiz). Ele cria o
database `TesteEscola`, as três tabelas e os dados de exemplo.

**SQL Server Express (configuração padrão do projeto):**

```powershell
sqlcmd -S ".\SQLEXPRESS" -U sa -P 1234 -C -i script-banco.sql
```

**LocalDB (alternativa, não exige instalação):**

```powershell
sqllocaldb start MSSQLLocalDB
sqlcmd -S "(localdb)\MSSQLLocalDB" -i script-banco.sql
```

**SSMS:** abra o arquivo, conecte na sua instância e execute (F5).

Conferindo:

```powershell
sqlcmd -S ".\SQLEXPRESS" -U sa -P 1234 -C -d TesteEscola -Q "SELECT COUNT(*) FROM dbo.Aluno;"
```

### 2. Connection string

Fica em [`src/TesteEscola.Api/Web.config`](src/TesteEscola.Api/Web.config), na
tag `<connectionStrings>`, nome **`TesteEscola`**:

```xml
<add name="TesteEscola"
     connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=TesteEscola;User ID=sa;Password=1234;Pooling=True;Encrypt=False;TrustServerCertificate=True;Application Name=TesteEscola.Api"
     providerName="System.Data.SqlClient" />
```

O padrão usa **SQL Server Express com autenticação SQL** (`sa` / `1234`) — é uma
senha de desenvolvimento local, sem valor fora daqui. Alternativas: troque para
`Integrated Security=True` (autenticação Windows) ou aponte o `Data Source` para
`(localdb)\MSSQLLocalDB` / `localhost`. Se a connection string faltar, a API sobe
e devolve erro claro na inicialização (não fica quebrando silenciosamente).

### 3. Subir a API

**Visual Studio:** abra `TesteEscola.sln`, defina `TesteEscola.Api` como
projeto de inicialização e rode (F5). Abre em `http://localhost:<porta>`.

**Linha de comando (MSBuild + IIS Express):**

```powershell
# na raiz do repositório
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" `
    TesteEscola.sln /t:restore,build /p:Configuration=Debug

& "C:\Program Files\IIS Express\iisexpress.exe" `
    /path:"$PWD\src\TesteEscola.Api" /port:8099
```

API em `http://localhost:8099`. Teste rápido:

```powershell
curl http://localhost:8099/api/turmas
```

> Restauração de pacotes: os projetos usam **PackageReference**, então
> `msbuild /t:restore` (ou o Visual Studio) baixa tudo. Não há `packages.config`.

### 4. Swagger

Com a API no ar, a documentação interativa (Swashbuckle) fica em:

```
http://localhost:8099/swagger
```

Documento OpenAPI cru em `http://localhost:8099/swagger/docs/v1`. Dá para
disparar as chamadas pelo próprio Swagger ("Try it out").

### 5. Abrir a tela de alunos

Com a API no ar, abra a raiz no navegador: `http://localhost:8099/`.
É uma página estática (`src/TesteEscola.Api/index.html` + `app/`) com HTML +
jQuery que consome `GET /api/alunos`: filtro por nome, tamanho de página,
paginação, "incluir inativos" e o total retornado pela API.

---

## Endpoints

Base: `http://localhost:8099`. Corpo e resposta em JSON (camelCase).
Também documentados no Swagger em `/swagger`.

### Alunos – CRUD

| Método | Rota | Sucesso | Erros |
|---|---|---|---|
| GET | `/api/alunos?nome=&pagina=1&tamanhoPagina=10&incluirInativos=false` | 200 | – |
| GET | `/api/alunos/{id}` | 200 | 404 |
| POST | `/api/alunos` | 201 + `Location` | 400 |
| PUT | `/api/alunos/{id}` | 200 | 400, 404 |
| DELETE | `/api/alunos/{id}` (exclusão lógica) | 204 | 404 |

`GET /api/alunos` devolve:

```json
{
  "itens": [ { "id": 1, "nome": "Ana Souza", "email": "...", "dataNascimento": "2006-03-14T00:00:00", "ativo": true, "dataCadastro": "..." } ],
  "total": 7,
  "pagina": 1,
  "tamanhoPagina": 10,
  "totalPaginas": 1
}
```

O total também vai no header `X-Total-Count`. Inativos ficam de fora a menos que
`incluirInativos=true`. `tamanhoPagina` é limitado a 100.

### Turmas

| Método | Rota | Sucesso |
|---|---|---|
| GET | `/api/turmas` | 200 |

Lista as turmas com `vagasRestantes` (coluna `VagasDisponiveis`):

```json
[ { "id": 1, "nome": "3A - Ensino Medio", "periodo": "Manha", "vagasTotal": 30, "vagasRestantes": 28 } ]
```

### Matrícula

| Método | Rota | Sucesso | Erros |
|---|---|---|---|
| POST | `/api/matriculas` | 201 + `Location` | 400 (corpo inválido), 404 (aluno/turma não existe), 409 (regra de negócio) |

Body: `{ "alunoId": 5, "turmaId": 1 }`. Devolve **409** quando a turma está sem
vaga, o aluno está inativo ou já está matriculado na turma:

```json
{ "mensagem": "A turma não possui vagas disponíveis.", "erros": [] }
```

### Relatório

| Método | Rota | Sucesso |
|---|---|---|
| GET | `/api/relatorios/alunos-por-turma` | 200 |

```json
[ { "turmaId": 1, "turma": "3A - Ensino Medio", "alunosMatriculados": 2, "vagasRestantes": 28 } ]
```

Consulta única, agregada no banco (sem montar nada em memória no C#):

```sql
SELECT t.Id AS TurmaId, t.Nome AS Turma, COUNT(m.Id) AS AlunosMatriculados,
       t.VagasDisponiveis AS VagasRestantes
FROM dbo.Turma t LEFT JOIN dbo.Matricula m ON m.TurmaId = t.Id
GROUP BY t.Id, t.Nome, t.VagasDisponiveis ORDER BY t.Nome;
```

---

## Regra de matrícula e transação

`MatriculaService.Criar` (camada de serviço) valida, na ordem:

1. `alunoId` e `turmaId` presentes → senão **400**.
2. Aluno existe → senão **404**.
3. Aluno está **ativo** → senão **409**.
4. Turma existe → senão **404**.
5. Turma tem vaga (`VagasDisponiveis > 0`) → senão **409**.
6. Não existe matrícula do aluno naquela turma → senão **409**.

Passando nas regras, `MatriculaRepository.CriarComTransacao` executa **numa
única transação**:

```sql
-- trava a linha da turma (serializa matrículas concorrentes na mesma turma)
SELECT VagasDisponiveis FROM dbo.Turma WITH (UPDLOCK, HOLDLOCK) WHERE Id = @turmaId;
-- revalida vaga e duplicidade sob o lock
INSERT INTO dbo.Matricula (AlunoId, TurmaId, DataMatricula) OUTPUT INSERTED.Id VALUES (...);
UPDATE dbo.Turma SET VagasDisponiveis = VagasDisponiveis - 1
 WHERE Id = @turmaId AND VagasDisponiveis > 0;   -- guarda extra
-- se qualquer passo falhar: ROLLBACK e exceção; senão COMMIT
```

Ou seja: as duas escritas (insert da matrícula + decremento da vaga) sempre
acontecem juntas, ou nenhuma acontece. As checagens no serviço dão o erro
amigável cedo; o repositório revalida sob lock para não abrir brecha de corrida
entre a checagem e o commit. O `UNIQUE (AlunoId, TurmaId)` no banco é a última
linha de defesa contra duplicidade.

---

## Decisões de arquitetura

- **Camadas.** Controller só faz HTTP (bind, status, `Location`). Regra fica em
  `TesteEscola.Services`. Acesso a dado fica em `TesteEscola.Infrastructure`.
  O `Domain` é o núcleo sem dependências (entidades, DTOs, interfaces, exceções).
- **Erros como exceções tipadas.** `NotFoundException`, `BusinessRuleException` e
  `ValidationException` herdam de `DomainException`. Um único
  `ExceptionFilterAttribute` as traduz em 404/409/400. Nenhum `try/catch` de
  status espalhado pelos controllers, e nada de 500 para erro previsível.
- **Validação em duas frentes.** `DataAnnotations` nos DTOs + `ModelState`
  pegam corpo malformado e campos obrigatórios (400 antes da action). As regras
  de fato (data no passado, e-mail, etc.) ficam no serviço, testáveis sem HTTP.
- **Dapper direto, sem repositório genérico.** SQL explícito, parametrizado,
  legível. `QueryMultiple` para trazer página + total numa ida só.
- **`IDbConnectionFactory`.** A infra cria `SqlConnection`; a connection string
  entra por injeção (a API lê do `Web.config`). Facilita teste e troca de origem.
- **DI com Autofac** (`Autofac.WebApi2`). Serviços/repositórios por
  _lifetime scope_ (por requisição); `IDbConnectionFactory` como singleton.
- **Formato de projeto.** Todos os projetos em csproj clássico
  (`TargetFrameworkVersion v4.8`) — é a stack legada pedida. Pacotes via
  **PackageReference** (restore pelo MSBuild, sem `packages.config`).

---

## Alterações no script do banco

Todas **aditivas** — o schema e os dados originais continuam iguais. Marcadas com
`-- [n]` no [`script-banco.sql`](script-banco.sql):

| # | Alteração | Motivo |
|---|---|---|
| 1 | `UNIQUE (AlunoId, TurmaId)` em `Matricula` | Garante no banco a regra "não pode matricular duas vezes na mesma turma" — última linha de defesa sob concorrência. |
| 2 | `CHECK (VagasDisponiveis >= 0 AND <= VagasTotal)` em `Turma` | Protege o invariante que a transação de matrícula mantém. |
| 3 | `CHECK (Periodo IN ('Manha','Tarde','Noite'))` em `Turma` | O domínio já estava no comentário da coluna; só tornei explícito. |
| 4 | `CREATE INDEX IX_Aluno_Nome ON Aluno(Nome)` | Suporta o filtro `LIKE` + `ORDER BY Nome` da listagem paginada. |

Nada disso muda contrato de API nem os dados de exemplo. Se preferir o script
literal do enunciado, basta remover as linhas marcadas — a aplicação funciona
igual (as mesmas regras são aplicadas em C#/SQL na transação).

---

## Itens bônus

Do enunciado, entreguei **um** dos três: a **tela HTML + jQuery** que consome a
listagem de alunos (`src/TesteEscola.Api/index.html` + `app/`).

Os outros dois — **cache com Redis** e **testes unitários** — **não foram
feitos**. Como eu faria cada um está descrito abaixo.

---

## O que ficou de fora e como eu faria

- **Testes unitários da regra de matrícula.** Não fiz. Faria com xUnit + Moq
  sobre `MatriculaService`, com os repositórios mockados, um caso por regra:
  aluno inexistente (404), aluno inativo (409), turma inexistente (404), turma
  sem vaga (409), matrícula duplicada (409) e caminho feliz (chama
  `CriarComTransacao` uma vez e devolve a resposta). A camada de serviço já foi
  desenhada para isso — sem dependência de HTTP nem de banco. Faria também testes
  de integração num SQL de teste (Respawn para limpar entre casos) cobrindo a
  transação de ponta a ponta.
- **Cache com Redis na listagem de turmas.** Não fiz. Criaria uma abstração
  `ICacheService` (`Get/Set/Remove`, TTL) no `Domain`, com `RedisCacheService`
  (`StackExchange.Redis`, `ConnectionMultiplexer` singleton) serializando em
  JSON; `TurmaService.Listar` consultaria o cache antes do banco e
  `MatriculaService.Criar` chamaria `Remove` da chave `turmas:listagem` logo após
  o commit, invalidando a listagem. Sem Redis no ambiente, entregaria uma
  implementação `MemoryCache` atrás da mesma interface, trocando só o registro no
  `DependencyInjectionConfig`.
- **Autenticação/autorização.** Não pedido. Colocaria um `DelegatingHandler` /
  filtro validando JWT e `[Authorize]` nos controllers.
- **Log estruturado.** Hoje erros inesperados caem no 500 padrão do Web API.
  Adicionaria Serilog num `IExceptionLogger` do Web API, com `CorrelationId`.
- **Paginação do relatório e mais relatórios.** O endpoint atual devolve todas as
  turmas (poucas linhas). Com volume, paginaria e daria filtro por período.
- **E-mail único de aluno.** Daria para um `UNIQUE` em `Aluno.Email` + tratamento
  de conflito (409) no `AlunoService`. Deixei de fora por não estar no enunciado
  e para não mudar a semântica dos dados de exemplo.
- **CI.** Um workflow rodando `msbuild` (e os testes, quando existirem) a cada push.
