"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ordinecliente_create_1 = require("./ordinecliente-create");
var articolo_create_1 = require("./articolo-create");
var packinglist_index_1 = require("./packinglist-index");
var ordinecliente_riepilogo_1 = require("./ordinecliente-riepilogo");
var ordinefornitore_create_1 = require("./ordinefornitore-create");
var ordineclientecommesso_editorderrows_1 = require("./ordineclientecommesso-editorderrows");
var ordineclientecommesso_riepilogo_1 = require("./ordineclientecommesso-riepilogo");
exports.arrayPageModules = [
    {
        page: "/OrdineCliente/Create",
        function: function (destination, template) { ordinecliente_create_1.attivatorePaginaCreate(); },
        menuItem: "navbarDropdown"
    },
    {
        page: "/OrdineClienteCommesso/EditOrderRows",
        function: function (destination, template) { ordineclientecommesso_editorderrows_1.attivatorePaginaOrdineClienteCommessoEditOrderRows(); },
        menuItem: "navbarDropdown"
    },
    {
        page: "/OrdineClienteCommesso/Riepilogo",
        function: function (destination, template) { ordineclientecommesso_riepilogo_1.attivatorePaginaOrdineClienteCommessoRiepilogo(); },
        menuItem: "navbarDropdown"
    },
    {
        page: "/OrdineCliente/Riepilogo",
        function: function (destination, template) { ordinecliente_riepilogo_1.attivatorePaginaOrdineClienteRiepilogo(); },
        menuItem: "navbarDropdown"
    },
    {
        page: "/Articolo/Create",
        function: function (destination, template) { articolo_create_1.attivatorePaginaCreateArticolo(); },
        menuItem: "navbarDropdown"
    },
    {
        page: "/PackingList/Index",
        function: function (destination, template) { packinglist_index_1.attivatorePaginaIndexPackingList(); },
        menuItem: "navbarDropdown"
    },
    {
        page: "/OrdineFornitore/Create",
        function: function (destination, template) { ordinefornitore_create_1.attivatorePaginaOrdineFornitoreCreate(); }
        //menuitem: "navbarDropdown"
    }
];
//# sourceMappingURL=sitemap.js.map