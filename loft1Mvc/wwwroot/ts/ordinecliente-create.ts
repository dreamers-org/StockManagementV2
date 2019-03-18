export function attivatorePaginaCreate() {
    window["getColorePerArticolo"] = getColorePerArticolo;
}

//ottiene la lista dei colori per il codice selezionato.
function getColorePerArticolo() {
    let txtCodice: JQuery<HTMLElement> = $("#dropdownCodiceArticolo");

    if (txtCodice != null) {
        let codice: string = txtCodice.val().toString();

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

        $.ajax({
            type: "POST",
            url: "/OrdineCliente/SelectDescrizioneFromCodice",
            data: { codice: codice },
            success: function (data) {
                console.log(data);
                $('#txtDescrizione').attr("value", data)
            }
        });

    }
}