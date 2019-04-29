export function attivatorePaginaOrdineFornitoreCreate() {
    window["getTxtValues"] = getTxtValues;

    window["verifyCorrectness"] = verifyCorrectness;

    window["getColorePerArticolo"] = getColorePerArticolo;
}


function getTxtValues() {
    let codice = $('#txtCodice').val();
    $.ajax({
        type: "POST",
        url: "/Articolo/getTxtValues",
        data: { codice: codice },
        success: function (data) {
            console.log(data);
            if (data.fornitore) {
                (<JQuery<HTMLInputElement>>$('#txtDescrizione')).val(data.descrizione);;
                (<JQuery<HTMLInputElement>>$('#txtPrezzoAcquisto')).val(data.prezzoAcquisto);;
                (<JQuery<HTMLInputElement>>$('#txtPrezzoVendita')).val(data.prezzoVendita);;
                (<JQuery<HTMLInputElement>>$('#txtTrancheConsegna')).val(data.trancheConsegna);;
                $('#ddlGenereProdotto').empty().append(new Option(data.genereProdotto, data.genereProdotto)).val(data.genereProdotto);
                $('#ddlTipoProdotto').empty().append(new Option(data.tipoProdotto, data.idTipoProdotto)).val(data.idTipoProdotto);
                $('#ddlFornitore').empty().append(new Option(data.fornitore, data.idFornitore)).val(data.idFornitore);
                $('#ddlCollezione').empty().append(new Option(data.collezione, data.idCollezione)).val(data.idCollezione);
                $('.form-control').attr("readonly", "readonly");
                (<JQuery<HTMLInputElement>>$('#txtColore')).removeAttr("readonly");
            }
        },
        error: function () {
            alert("Errore");
        }
    })
};

function verifyCorrectness() {
    let codice = $('#txtCodice').val();
    let colore = $('#txtColore').val();
    $.ajax({
        type: "POST",
        url: "/Articolo/verifyCorrectness",
        data: { codice: codice, colore: colore },
        success: function (data) {
            console.log(data);
            if (!(data)) {
                $('#divError').show();
                $('#btnInserisci').attr("disabled", "disabled");
            }
            else {
                $('#divError').hide();
                $('#btnInserisci').removeAttr("disabled");
            }
        },
        error: function () {
            alert("Errore");
        }
    })
};

//ottiene la lista dei colori per il codice selezionato.
function getColorePerArticolo() {
    let txtCodice: JQuery<HTMLElement> = $("#txtCodice");

    if (txtCodice != null) {
        let codice: string = txtCodice.val().toString();
        $.ajax({
            type: "POST",
            url: "/OrdineCliente/SelectColoriFromCodice",
            data: { codice: codice },
            success: function (data) {
                console.log(data.length);
                $('#ddlColore').removeAttr("disabled");
                if (data.length == 0) {
                    var s = '<option value="-1">Seleziona un colore</option>';
                    $("#dropdownColore").html(s);
                }
                else {
                    for (var i = 0; i < data.length; i++) {
                        var s = '<option value="-1">Seleziona un colore</option>';
                        for (var i = 0; i < data.length; i++) {
                            console.log(data[i]);
                            s += '<option value="' + data[i].colore + '">' + data[i].colore + '</option>';
                        }
                        $("#dropdownColore").html(s);
                        $("#dropdownColore").removeAttr("disabled");
                    }
                }
            },
            error: function () {
                console.log("Errore");
            }
        });
    }
}