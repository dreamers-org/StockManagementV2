import { attivatorePaginaCreate } from "./create";
import { attivatorePaginaCreateArticolo } from "./create-articolo";

export interface pageModule {
    page?: string;
    function?: (destination?: JQuery, template?: any) => void
    menuItem?: string;
}

export var arrayPageModules: pageModule[] = [
    {
        page: "/OrdineCliente/Create",
        function: function (destination, template) { attivatorePaginaCreate() },
        menuItem: "navbarDropdown"
    },
    {
        page: "/Articolo/Create",
        function: function (destination, template) { attivatorePaginaCreateArticolo() },
        menuItem: "navbarDropdown"
    }

];


