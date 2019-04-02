﻿//variabile globale usata per gestire gli articoli disponibili.
var articoliDisponibili: Array<String> = null; 

export function attivatorePaginaCreate() {
    window["getColorePerArticolo"] = getColorePerArticolo;

    window["caricaArticoli"] = caricaArticoli;

    window["getTaglieDisponibiliArticolo"] = getTaglieDisponibiliArticolo;

    //carico la combo degli articoli
    caricaArticoli();

    //nascondo il div d'errore.
    $("#divAlertError").hide();
}

//ottiene la lista dei colori per il codice selezionato.
function getColorePerArticolo() {
    let txtCodice: JQuery<HTMLElement> = $("#txtCodiceArticolo");
    $("#dropdownColore").removeAttr("readonly");
    $("#dropdownColore").empty();
    $('#txtDescrizione').empty();

    //nascondo il div d'errore.
    $("#divAlertError").hide();
    if (txtCodice != null && txtCodice.val().toString() != "") {
        let codice: string = txtCodice.val().toString();

        if (jQuery.inArray(codice, articoliDisponibili) != -1) {
            //carico la lista dei colori.
            $.ajax({
                type: "POST",
                url: "/OrdineCliente/SelectColoriFromCodice",
                data: { codice: codice },
                success: function (data) {
                    console.log(data.length);
                    $('#dropdownColore').removeAttr("disabled");
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

            //carico la descrizione dell'articolo.
            $.ajax({
                type: "POST",
                url: "/OrdineCliente/SelectDescrizioneFromCodice",
                data: { codice: codice },
                success: function (data) {
                    console.log(data);
                    $('#txtDescrizione').attr("value", data)
                }
            });

          
        } else {
            $("#dropdownColore").attr("readonly","readonly");
            $("#divAlertError").show();
            $("#divAlertError").text("L'articolo non esiste o non è disponibile per la data di consegna selezionata.");
        }
    }
}

//carica gli articoli in base alla data di consegna selezionata.
function caricaArticoli() {
    try {
        let txtDataConsegna: JQuery<HTMLElement> = $("#txtDataConsegna");

        if (txtDataConsegna != null) {
            let valoreDataConsegna: unknown = txtDataConsegna.val().toString();

            if (valoreDataConsegna != null && valoreDataConsegna != "") {
                let dataConsegna: Date = <Date>valoreDataConsegna;

                $.ajax({
                    type: "POST",
                    url: "/OrdineCliente/SelectCodiciArticoli",
                    data: { dataConsegna: dataConsegna },

                    success: function (data) {
                        console.log("caricamento articoli");
                        //let txtCodiceArticolo:  JQuery<HTMLElement> =/*;*/
                        articoliDisponibili= data;
                        $("#txtCodiceArticolo").removeAttr("readonly");
                        (<any>$("#txtCodiceArticolo")).autocomplete({ source: articoliDisponibili  });
                    },
                    error: function () {
                        console.log("Errore caricaArticoli");
                    }
                });
            }
        }
    } catch (e) {
        console.log("Errore caricaArticoli");
    }
}

function getTaglieDisponibiliArticolo() {
    let codice = $('#txtCodiceArticolo').val();
    let colore = $('#dropdownColore').val()
    $.ajax({
        type: "POST",
        url: "/Articolo/getTaglieDisponibili",
        data: { codice: codice, colore: colore },
        success: function (data) {
            console.log(data);
            if (data.xxs) {
                (<JQuery<HTMLInputElement>>$('#txtXxs')).val("0");
                (<JQuery<HTMLInputElement>>$('#txtXxs')).attr("readonly","readonly");
            }
            else {
                (<JQuery<HTMLInputElement>>$('#txtXxs')).removeAttr("readonly");
            }
            if (data.xs) {
                (<JQuery<HTMLInputElement>>$('#txtXs')).val("0");
                (<JQuery<HTMLInputElement>>$('#txtXs')).attr("readonly", "readonly");
            }
            else {
                (<JQuery<HTMLInputElement>>$('#txtXs')).removeAttr("readonly");
            }
            if (data.s) {
                (<JQuery<HTMLInputElement>>$('#txtS')).val("0");
                (<JQuery<HTMLInputElement>>$('#txtS')).attr("readonly", "readonly");
            }
            else {
                (<JQuery<HTMLInputElement>>$('#txtS')).removeAttr("readonly");
            }
            if (data.m) {
                (<JQuery<HTMLInputElement>>$('#txtM')).val("0");
                (<JQuery<HTMLInputElement>>$('#txtM')).attr("readonly", "readonly");
            }
            else {
                (<JQuery<HTMLInputElement>>$('#txtM')).removeAttr("readonly");
            }
            if (data.l) {
                (<JQuery<HTMLInputElement>>$('#txtL')).val("0");
                (<JQuery<HTMLInputElement>>$('#txtL')).attr("readonly", "readonly");
            }
            else {
                (<JQuery<HTMLInputElement>>$('#txtL')).removeAttr("readonly");
            }
            if (data.xl) {
                (<JQuery<HTMLInputElement>>$('#txtXl')).val("0");
                (<JQuery<HTMLInputElement>>$('#txtXl')).attr("readonly", "readonly");
            }
            else {
                (<JQuery<HTMLInputElement>>$('#txtXl')).removeAttr("readonly");
            }
            if (data.xxl) {
                (<JQuery<HTMLInputElement>>$('#txtXxl')).val("0");
                (<JQuery<HTMLInputElement>>$('#txtXxl')).attr("readonly", "readonly");
            }
            else {
                (<JQuery<HTMLInputElement>>$('#txtXxl')).removeAttr("readonly");
            }
            if (data.xxxl) {
                (<JQuery<HTMLInputElement>>$('#txtXxxl')).val("0");
                (<JQuery<HTMLInputElement>>$('#txtXxxl')).attr("readonly", "readonly");
            }
            else {
                (<JQuery<HTMLInputElement>>$('#txtXxxl')).removeAttr("readonly");
            }
            if (data.tagliaUnica) {
                (<JQuery<HTMLInputElement>>$('#txtTagliaUnica')).val("0");
                (<JQuery<HTMLInputElement>>$('#txtTagliaUnica')).attr("readonly", "readonly");
            }
            else {
                (<JQuery<HTMLInputElement>>$('#txtTagliaUnica')).removeAttr("readonly");
            }
        },
        error: function () {
            alert("Errore");
        }
    })
};