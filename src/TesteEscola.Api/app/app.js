/* global $ */
(function () {
    "use strict";

    var estado = { pagina: 1, totalPaginas: 1 };

    function fmtData(iso) {
        if (!iso) return "";
        var d = new Date(iso);
        return isNaN(d) ? iso : d.toLocaleDateString("pt-BR");
    }

    function carregar() {
        var params = {
            nome: $("#nome").val().trim(),
            pagina: estado.pagina,
            tamanhoPagina: $("#tamanhoPagina").val(),
            incluirInativos: $("#incluirInativos").is(":checked")
        };
        if (!params.nome) delete params.nome;

        $("#status").text("Carregando...").removeClass("erro");

        $.getJSON("/api/alunos", params)
            .done(function (resp) {
                var itens = resp.itens || [];
                var linhas = itens.map(function (a) {
                    return "<tr class='" + (a.ativo ? "" : "inativo") + "'>" +
                        "<td>" + a.id + "</td>" +
                        "<td>" + $("<div>").text(a.nome).html() + "</td>" +
                        "<td>" + $("<div>").text(a.email).html() + "</td>" +
                        "<td>" + fmtData(a.dataNascimento) + "</td>" +
                        "<td>" + (a.ativo ? "sim" : "não") + "</td>" +
                        "<td>" + fmtData(a.dataCadastro) + "</td>" +
                        "</tr>";
                }).join("");

                $("#tabela tbody").html(linhas ||
                    "<tr><td colspan='6' class='vazio'>Nenhum aluno encontrado.</td></tr>");

                estado.totalPaginas = resp.totalPaginas || 1;
                $("#status").text("Total: " + resp.total + " aluno(s).");
                $("#infoPagina").text("Página " + resp.pagina + " de " + estado.totalPaginas);
                $("#anterior").prop("disabled", resp.pagina <= 1);
                $("#proxima").prop("disabled", resp.pagina >= estado.totalPaginas);
            })
            .fail(function (xhr) {
                var msg = "Erro " + xhr.status;
                try { msg += ": " + (JSON.parse(xhr.responseText).mensagem || ""); } catch (e) { }
                $("#status").text(msg).addClass("erro");
                $("#tabela tbody").empty();
            });
    }

    $("#filtro").on("submit", function (e) {
        e.preventDefault();
        estado.pagina = 1;
        carregar();
    });
    $("#tamanhoPagina, #incluirInativos").on("change", function () {
        estado.pagina = 1;
        carregar();
    });
    $("#anterior").on("click", function () {
        if (estado.pagina > 1) { estado.pagina--; carregar(); }
    });
    $("#proxima").on("click", function () {
        if (estado.pagina < estado.totalPaginas) { estado.pagina++; carregar(); }
    });

    carregar();
})();
