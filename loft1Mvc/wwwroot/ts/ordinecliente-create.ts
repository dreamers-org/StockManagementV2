//variabile globale usata per gestire gli articoli disponibili.
var articoliDisponibili: Array<String> = null; 

export function attivatorePaginaCreate() {
    window["getColorePerArticolo"] = getColorePerArticolo;

    window["caricaArticoli"] = caricaArticoli;

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
    if (txtCodice != null) {
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

            $("#divAlertError").hide();
        } else {
            $("#dropdownColore").attr("readonly","readonly");
            $("#divAlertError").show();
            $("#divAlertError").text("L'articolo non è disponibile per la data consegna selezionata.");
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