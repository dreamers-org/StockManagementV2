export function attivatorePaginaOrdineClienteRiepilogo() {
    window["abilitaConcludiOrdine"] = abilitaConcludiOrdine;
}

//Abilita il pulsante "concludi" in base allo stato della checkbox
function abilitaConcludiOrdine(checkbox:HTMLInputElement) {
    if (checkbox != null) {
        //ottiene il bottone
        let btnConcludiOrdine: HTMLElement = document.getElementById("btnConcludiOrdine");
        if (btnConcludiOrdine != null) {
            //se checked =true allora abilito il pulsante altrimenti lo disabilito.
            if (checkbox.checked) {
                btnConcludiOrdine.removeAttribute("disabled");
            } else {
                btnConcludiOrdine.setAttribute("disabled", "disabled");
            }

            //cambio il value della checkbox in modo da poterlo "mandare" al controller.
            checkbox.setAttribute("value", checkbox.checked.toString());
        }
    }
}
