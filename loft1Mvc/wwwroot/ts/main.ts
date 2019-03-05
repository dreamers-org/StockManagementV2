//import delle librerie
import * as $ from "jquery";
import "popper.js";
import "bootstrap";
import "@fortawesome/fontawesome-free";

//import librerie interne
import { arrayPageModules } from './sitemap';

//import '../js/script.js';
//import '../js/site.js';

//import dei css
import 'bootstrap/dist/css/bootstrap.min.css';
import '../css/site.css';

$(document).ready(function () {
    try {
        //ottengo l'url corrente.
        let currentUrl: string = window.location.pathname;

        console.log("CurrentUrl" + currentUrl);

        if (currentUrl == "/") {
            $("#pageHome").addClass("active");
        } else {
            //in base alla pagina corrente attivo la funzione corretta.
            for (let i = 0; i < arrayPageModules.length; i++) {
                if (currentUrl.indexOf(arrayPageModules[i].page) !== -1) {
                    if (arrayPageModules[i].function) {
                        arrayPageModules[i].function();
                    }
                    //attivo l'item corretto del menu.
                    attivaMenuItemCorrente(arrayPageModules[i].menuItem);
                }
            }
        }

    } catch (ex) {
        //let err: Errore = new Errore
        //err.tracciaErrore(ex, "document.ready_main.ts");
        console.log("errore:");
    }
});

function attivaMenuItemCorrente(idMenuItem: string) {
    console.log(idMenuItem);
    $(`#${idMenuItem}`).addClass("active");
}

