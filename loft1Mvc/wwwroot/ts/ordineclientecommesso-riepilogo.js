"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function attivatorePaginaOrdineClienteCommessoRiepilogo() {
    window["abilitaConcludiOrdine"] = abilitaConcludiOrdine;
}
exports.attivatorePaginaOrdineClienteCommessoRiepilogo = attivatorePaginaOrdineClienteCommessoRiepilogo;
//Abilita il pulsante "concludi" in base allo stato della checkbox
function abilitaConcludiOrdine(checkbox) {
    if (checkbox != null) {
        //ottiene il bottone
        var btnConcludiOrdine = document.getElementById("btnConcludiOrdine");
        if (btnConcludiOrdine != null) {
            //se checked =true allora abilito il pulsante altrimenti lo disabilito.
            if (checkbox.checked) {
                btnConcludiOrdine.removeAttribute("disabled");
            }
            else {
                btnConcludiOrdine.setAttribute("disabled", "disabled");
            }
            //cambio il value della checkbox in modo da poterlo "mandare" al controller.
            checkbox.setAttribute("value", checkbox.checked.toString());
        }
    }
}
//# sourceMappingURL=ordineclientecommesso-riepilogo.js.map