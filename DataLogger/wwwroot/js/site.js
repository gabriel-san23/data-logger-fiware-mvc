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

    carregarLuminosidade(serverIp, idDispositivo, lastN);
    carregarTemperatura(serverIp, idDispositivo, lastN);
    carregarUmidade(serverIp, idDispositivo, lastN);
}

function carregarLuminosidade(serverIp, idDispositivo, lastN) {
    $.ajax({
        type: "GET",
        url: "/Home/historicoLuminosidade",
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
    $.ajax({
        type: "GET",
        url: "/Home/historicoTemperatura",
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
    $.ajax({
        type: "GET",
        url: "/Home/historicoHumidade",
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
        // O controller retorna um objeto com "sucesso" e "dados" (que é outro JSON em string)
        var wrapper = typeof respostaJson === "string" ? JSON.parse(respostaJson) : respostaJson;

        if (!wrapper.sucesso) {
            console.log("Fiware retornou erro:", wrapper.dados);
            return null;
        }

        // O campo "dados" é o JSON do STH-Comet em string
        var dadosFiware = typeof wrapper.dados === "string" ? JSON.parse(wrapper.dados) : wrapper.dados;

        var valores = dadosFiware
            .contextResponses[0]
            .contextElement
            .attributes[0]
            .values;

        if (!valores || valores.length === 0)
            return null;

        // Separa os horários (labels) e os valores para o Chart.js
        var labels = valores.map(function (v) {
            // Formata a data/hora para exibição no gráfico
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

    // Destroi o gráfico anterior se existir (evita sobreposição)
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
                title: {
                    display: true,
                    text: titulo
                }
            },
            scales: {
                y: { beginAtZero: false }
            }
        }
    });
}
