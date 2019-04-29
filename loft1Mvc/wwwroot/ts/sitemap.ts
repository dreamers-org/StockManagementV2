import { attivatorePaginaCreate } from "./ordinecliente-create";
import { attivatorePaginaCreateArticolo } from "./articolo-create";
import { attivatorePaginaIndexPackingList } from "./packinglist-index";
import { attivatorePaginaOrdineClienteRiepilogo } from "./ordinecliente-riepilogo"
import { attivatorePaginaOrdineFornitoreCreate } from "./ordinefornitore-create"

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
        page: "/OrdineCliente/Riepilogo",
        function: function (destination, template) { attivatorePaginaOrdineClienteRiepilogo() },
        menuItem: "navbarDropdown"
    },
    {
        page: "/Articolo/Create",
        function: function (destination, template) { attivatorePaginaCreateArticolo() },
        menuItem: "navbarDropdown"
    },
    {
        page: "/PackingList/Index",
        function: function (destination, template) { attivatorePaginaIndexPackingList() },
        menuItem: "navbarDropdown"
    },
    {
        page: "/OrdineFornitore/Create",
        function: function (destination, template) { attivatorePaginaOrdineFornitoreCreate() }
        //menuitem: "navbarDropdown"
    }
];


