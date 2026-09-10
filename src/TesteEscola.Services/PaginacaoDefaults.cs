namespace TesteEscola.Services
{
    internal static class PaginacaoDefaults
    {
        public const int PaginaInicial = 1;
        public const int TamanhoPadrao = 10;
        public const int TamanhoMaximo = 100;

        public static int NormalizarPagina(int pagina)
        {
            return pagina < 1 ? PaginaInicial : pagina;
        }

        public static int NormalizarTamanho(int tamanho)
        {
            if (tamanho <= 0) return TamanhoPadrao;
            return tamanho > TamanhoMaximo ? TamanhoMaximo : tamanho;
        }
    }
}
