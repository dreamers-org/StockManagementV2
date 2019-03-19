"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
//import delle librerie
var $ = require("jquery");
require("jquery-ui");
require("jquery-autocomplete");
require("popper.js");
require("bootstrap");
require("@fortawesome/fontawesome-free");
require("jquery-validation");
require("jquery-validation-unobtrusive");
//import librerie interne
var sitemap_1 = require("./sitemap");
//import dei css
require("bootstrap/dist/css/bootstrap.min.css");
require("../css/site.css");
$(document).ready(function () {
    try {
        //ottengo l'url corrente.
        var currentUrl = window.location.pathname;
        console.log("CurrentUrl" + currentUrl);
        if (currentUrl == "/") {
            $("#pageHome").addClass("active");
        }
        else {
            //in base alla pagina corrente attivo la funzione corretta.
            for (var i = 0; i < sitemap_1.arrayPageModules.length; i++) {
                if (currentUrl.indexOf(sitemap_1.arrayPageModules[i].page) !== -1) {
                    if (sitemap_1.arrayPageModules[i].function) {
                        console.log("test: " + sitemap_1.arrayPageModules[i].function.toString());
                        sitemap_1.arrayPageModules[i].function();
                    }
                    //attivo l'item corretto del menu.
                    console.log("attivatore:" + sitemap_1.arrayPageModules[i].menuItem);
                    attivaMenuItemCorrente(sitemap_1.arrayPageModules[i].menuItem);
                }
            }
        }
    }
    catch (ex) {
        //let err: Errore = new Errore
        //err.tracciaErrore(ex, "document.ready_main.ts");
        console.log("errore:");
    }
});
function attivaMenuItemCorrente(idMenuItem) {
    console.log(idMenuItem);
    //$(`#${idMenuItem}`).addClass("active");
}
//# sourceMappingURL=main.js.map