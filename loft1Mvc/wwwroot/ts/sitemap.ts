//import { attivatorePaginaCalendario } from "./pagina-calendario";
//import { attivatorePaginaRicercaOrdine } from "./ricerca-ordini";

export interface pageModule {
    page?: string;
    function?: (destination?: JQuery, template?: any) => void
    menuItem?: string;
}

export var arrayPageModules: pageModule[] = [
    {
        page: "/Articolo",
        //function: function (destination, template) { attivatorePaginaCalendario() },
        menuItem: "navbarDropdown"
    },
    {
        page: "/ArticoloAnnullato",
        //function: function (destination, template) { attivatorePaginaCalendario() },
        menuItem: "navbarDropdown"
    },
    {
        page: "/OrdineDalCliente",
        //function: function (destination, template) { attivatorePaginaCalendario() },
        menuItem: "navbarDropdown"
    },
];


