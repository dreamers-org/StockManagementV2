"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var create_1 = require("./create");
var create_articolo_1 = require("./create-articolo");
var index_packinglist_1 = require("./index-packinglist");
exports.arrayPageModules = [
    {
        page: "/OrdineCliente/Create",
        function: function (destination, template) { create_1.attivatorePaginaCreate(); },
        menuItem: "navbarDropdown"
    },
    {
        page: "/Articolo/Create",
        function: function (destination, template) { create_articolo_1.attivatorePaginaCreateArticolo(); },
        menuItem: "navbarDropdown"
    },
    {
        page: "/PackingList/Index",
        function: function (destination, template) { index_packinglist_1.attivatorePaginaIndexPackingList(); },
        menuItem: "navbarDropdown"
    }
];
//# sourceMappingURL=sitemap.js.map