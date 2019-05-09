import { attivatorePaginaCreate } from "./ordinecliente-create";
import { attivatorePaginaCreateArticolo } from "./articolo-create";
import { attivatorePaginaIndexPackingList } from "./packinglist-index";
import { attivatorePaginaOrdineClienteRiepilogo } from "./ordinecliente-riepilogo"
import { attivatorePaginaOrdineFornitoreCreate } from "./ordinefornitore-create"
import { attivatorePaginaOrdineClienteCommessoEditOrderRows } from "./ordineclientecommesso-editorderrows"
import { attivatorePaginaOrdineClienteCommessoRiepilogo } from "./ordineclientecommesso-riepilogo"

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
        page: "/OrdineClienteCommesso/EditOrderRows",
        function: function (destination, template) { attivatorePaginaOrdineClienteCommessoEditOrderRows() },
        menuItem: "navbarDropdown"
    },
    {
        page: "/OrdineClienteCommesso/Riepilogo",
        function: function (destination, template) { attivatorePaginaOrdineClienteCommessoRiepilogo() },
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


