//import delle librerie
import * as $ from "jquery";
import "jquery-ui";
import "jquery-autocomplete";
import "popper.js";
import "bootstrap";
import "@fortawesome/fontawesome-free";
import "jquery-validation"; 
import "jquery-validation-unobtrusive";

//import librerie interne
import { arrayPageModules } from './sitemap';

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
                        console.log("test: " + arrayPageModules[i].function.toString());
                        arrayPageModules[i].function();
                    }
                    //attivo l'item corretto del menu.

                    console.log("attivatore:" + arrayPageModules[i].menuItem)
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
    //$(`#${idMenuItem}`).addClass("active");
}
