import { attivatorePaginaCreate } from "./create";

export interface pageModule {
    page?: string;
    function?: (destination?: JQuery, template?: any) => void
    menuItem?: string;
}

export var arrayPageModules: pageModule[] = [
    {
        page: "/OrdineDalCliente/Create",
        function: function (destination, template) { attivatorePaginaCreate() },
        menuItem: "navbarDropdown"
    }
];


