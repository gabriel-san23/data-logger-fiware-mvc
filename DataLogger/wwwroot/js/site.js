const text =
    "E se você pudesse produzir o queijo perfeito?";

const typingText = document.getElementById("typing-text");

let index = 0;

function typeWriter() {
    if (index < text.length) {
        typingText.innerHTML += text.charAt(index);
        index++;
        setTimeout(typeWriter, 60);
    }
}

window.onload = typeWriter;

var graficoLuminosidade = null;
var graficoTemperatura = null;
var graficoUmidade = null;

function carregarGraficos() {
    var serverIp = $("#serverIp").val();
    var idDispositivo = $("#idDispositivo").val();
    var lastN = $("#lastN").val() || 30;

    if (!serverIp || !idDispositivo) {
        alert("Preencha o IP do servidor e selecione um dispositivo.");
        return;
    }

    var reqLum = carregarLuminosidade(serverIp, idDispositivo, lastN);
    var reqTemp = carregarTemperatura(serverIp, idDispositivo, lastN);
    var reqUmi = carregarUmidade(serverIp, idDispositivo, lastN);

    $.when(reqLum, reqTemp, reqUmi).always(function (resLum, resTemp, resUmi) {

        var jsonLum = resLum[0];
        var jsonTemp = resTemp[0];
        var jsonUmi = resUmi[0];

        var dadosLum = extrairValoresDoFiware(jsonLum, "luminosity");
        var dadosTemp = extrairValoresDoFiware(jsonTemp, "temperature");
        var dadosUmi = extrairValoresDoFiware(jsonUmi, "humidity");

        if (dadosLum && dadosTemp && dadosUmi) {
            var lote = {
                Luminosidades: dadosLum.valores.map(function (v) { return Math.round(v); }),
                Temperaturas: dadosTemp.valores,
                Umidades: dadosUmi.valores.map(function (v) { return Math.round(v); })
            };

            $.ajax({
                type: "POST",
                url: "/Dashboard/salvarLote?idDispositivo=" + idDispositivo,
                contentType: "application/json",
                data: JSON.stringify(lote),
                success: function () {
                    carregarTabelaRegistros();
                },
                error: function () {
                    $("#statusTabela").text("Erro ao salvar registros no banco.");
                }
            });
        } else {
            carregarTabelaRegistros();
        }
    });
}

function carregarLuminosidade(serverIp, idDispositivo, lastN) {
    return $.ajax({
        type: "GET",
        url: "/Dashboard/historicoLuminosidade",
        data: { serverIp: serverIp, idDispositivo: idDispositivo, lastN: lastN },
        success: function (resposta) {
            var dados = extrairValoresDoFiware(resposta, "luminosity");
            if (dados) {
                renderizarGrafico("canvasLuminosidade", "Luminosidade", dados.labels, dados.valores, "rgb(255, 206, 86)", "graficoLuminosidade");
            } else {
                $("#statusLuminosidade").text("Sem dados de luminosidade.");
            }
        },
        error: function () {
            $("#statusLuminosidade").text("Erro ao buscar luminosidade.");
        }
    });
}

function carregarTemperatura(serverIp, idDispositivo, lastN) {
    return $.ajax({
        type: "GET",
        url: "/Dashboard/historicoTemperatura",
        data: { serverIp: serverIp, idDispositivo: idDispositivo, lastN: lastN },
        success: function (resposta) {
            var dados = extrairValoresDoFiware(resposta, "temperature");
            if (dados) {
                renderizarGrafico("canvasTemperatura", "Temperatura (°C)", dados.labels, dados.valores, "rgb(255, 99, 132)", "graficoTemperatura");
            } else {
                $("#statusTemperatura").text("Sem dados de temperatura.");
            }
        },
        error: function () {
            $("#statusTemperatura").text("Erro ao buscar temperatura.");
        }
    });
}

function carregarUmidade(serverIp, idDispositivo, lastN) {
    return $.ajax({
        type: "GET",
        url: "/Dashboard/historicoHumidade",
        data: { serverIp: serverIp, idDispositivo: idDispositivo, lastN: lastN },
        success: function (resposta) {
            var dados = extrairValoresDoFiware(resposta, "humidity");
            if (dados) {
                renderizarGrafico("canvasUmidade", "Umidade (%)", dados.labels, dados.valores, "rgb(54, 162, 235)", "graficoUmidade");
            } else {
                $("#statusUmidade").text("Sem dados de umidade.");
            }
        },
        error: function () {
            $("#statusUmidade").text("Erro ao buscar umidade.");
        }
    });
}

function extrairValoresDoFiware(respostaJson, nomeAtributo) {
    try {
        var wrapper = typeof respostaJson === "string" ? JSON.parse(respostaJson) : respostaJson;

        if (!wrapper.sucesso) {
            console.log("Fiware retornou erro:", wrapper.dados);
            return null;
        }

        var dadosFiware = typeof wrapper.dados === "string" ? JSON.parse(wrapper.dados) : wrapper.dados;

        var valores = dadosFiware
            .contextResponses[0]
            .contextElement
            .attributes[0]
            .values;

        if (!valores || valores.length === 0)
            return null;

        var labels = valores.map(function (v) {
            var d = new Date(v.recvTime);
            return d.getHours() + ":" + String(d.getMinutes()).padStart(2, "0") + ":" + String(d.getSeconds()).padStart(2, "0");
        });

        var numeros = valores.map(function (v) {
            return parseFloat(v.attrValue);
        });

        return { labels: labels, valores: numeros };

    } catch (e) {
        console.error("Erro ao extrair dados do Fiware:", e);
        return null;
    }
}

function renderizarGrafico(canvasId, titulo, labels, valores, cor, varGrafico) {
    var ctx = document.getElementById(canvasId);
    if (!ctx) return;

    if (window[varGrafico]) {
        window[varGrafico].destroy();
    }

    window[varGrafico] = new Chart(ctx, {
        type: "line",
        data: {
            labels: labels,
            datasets: [{
                label: titulo,
                data: valores,
                borderColor: cor,
                backgroundColor: cor.replace("rgb", "rgba").replace(")", ", 0.2)"),
                borderWidth: 2,
                fill: true,
                tension: 0.3
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { display: true },
                title: { display: true, text: titulo }
            },
            scales: {
                y: { beginAtZero: false }
            }
        }
    });
}

function carregarTabelaRegistros() {
    var idDispositivo = $("#idDispositivo").val();
    var lastN = $("#lastN").val() || 30;
    var ordem = $("#filtroOrdem").val() || "desc";
    var filtroParametro = $("#filtroParametro").val() || "todos";

    if (!idDispositivo) {
        $("#statusTabela").text("Informe o ID do dispositivo para carregar os registros.");
        return;
    }

    $("#statusTabela").text("Carregando registros...");

    $.ajax({
        type: "GET",
        url: "/Dashboard/listaRegistros",
        data: {
            idDispositivo: idDispositivo,
            lastN: lastN,
            ordem: ordem,
            filtroParametro: filtroParametro
        },
        success: function (resposta) {
            var resultado = typeof resposta === "string" ? JSON.parse(resposta) : resposta;

            if (!resultado.sucesso) {
                $("#statusTabela").text("Erro ao carregar registros: " + resultado.mensagem);
                return;
            }

            var registros = resultado.dados;
            $("#statusTabela").text("");

            if (!registros || registros.length === 0) {
                $("#corpoTabelaRegistros").html(
                    "<tr><td colspan='6' class='text-center text-muted'>Nenhum registro encontrado.</td></tr>"
                );
                $("#painelInformacoesAdicionais").hide();
                return;
            }

            var linhas = "";
            for (var i = 0; i < registros.length; i++) {
                var r = registros[i];
                linhas += "<tr>";
                linhas += "<td>" + r.id + "</td>";
                linhas += "<td>" + (r.descricaoDispositivo || "-") + "</td>";
                linhas += "<td>" + r.dataHora + "</td>";
                linhas += "<td>" + r.valorLuminosidade + "</td>";
                linhas += "<td>" + r.valorTemperatura + "</td>";
                linhas += "<td>" + r.valorUmidade + "</td>";
                linhas += "</tr>";
            }
            $("#corpoTabelaRegistros").html(linhas);

            var info = resultado.informacoesAdicionais;
            if (info) {
                $("#mediaLuminosidade").text(info.mediaLuminosidade);
                $("#maiorLuminosidade").text(info.maiorLuminosidade);
                $("#menorLuminosidade").text(info.menorLuminosidade);
                $("#mediaTemperatura").text(info.mediaTemperatura);
                $("#maiorTemperatura").text(info.maiorTemperatura);
                $("#menorTemperatura").text(info.menorTemperatura);
                $("#mediaUmidade").text(info.mediaUmidade);
                $("#maiorUmidade").text(info.maiorUmidade);
                $("#menorUmidade").text(info.menorUmidade);
                $("#painelInformacoesAdicionais").show();
            }
        },
        error: function () {
            $("#statusTabela").text("Erro ao buscar registros do banco de dados.");
        }
    });
}