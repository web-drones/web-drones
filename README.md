# web-drones
Sistema para automatizar la venta y renta de drones 

## Descripción general

Web Drones es una aplicación web desarrollada con ASP.NET Core Blazor enfocada en la venta y gestión de drones. El sistema permite a los usuarios explorar un catálogo de productos, ver detalles de cada dron, agregarlos a un carrito de compras y generar pedidos. El proceso de compra está orientado a una comunicación directa con el vendedor (por ejemplo, mediante WhatsApp), en lugar de integrar pasarelas de pago en línea.

El proyecto fue diseñado con un enfoque académico y práctico, priorizando la claridad del flujo de compra, la validación de disponibilidad de productos y una interfaz moderna.

## Tecnologías utilizadas

1). ASP.NET Core
2). Blazor Server (Interactive Server Render Mode)
3). MudBlazor (UI Components)
4). SQL Server
5). Entity Framework Core
6). C#
7). HTML / CSS

## Funcionalidades principales

1). Catálogo de drones con información detallada
2). Página de detalles de producto
3). Sistema de carrito de compras
4). Validación de stock y disponibilidad
5). Mensajes de estado (producto agregado, no disponible, sin stock)
6). Generación de pedido sin pasarela de pago
7). Envío de pedido mediante enlace a WhatsApp
8). Páginas informativas como About / Sobre Nosotros

## Flujo general del sistema

1). El usuario ingresa a la página principal.
2). Explora el catálogo de drones disponibles.
3). Selecciona un dron para ver sus detalles.
4). Agrega el producto al carrito, validando stock y disponibilidad.
5). Accede al carrito para revisar los productos seleccionados.
6). Confirma la compra.
7). El sistema genera un resumen del pedido y redirige a WhatsApp para completar la compra.

## Estados al agregar al carrito

El sistema maneja distintos resultados al intentar agregar un producto al carrito:

1). Success: Producto agregado correctamente.
2). OutOfStock: El producto no tiene stock disponible.
3). NotAvailableForThisOperation: El producto no está disponible para esa operación.
4). NotAvailable: El producto no está disponible en general.

Estos estados se utilizan tanto en la página principal como en la vista de detalles.

## Base de datos

La base de datos está diseñada para soportar usuarios, roles, productos y pedidos.

### Elementos principales

1). Base de datos: `WEBDRONES`
2). Tabla de roles
3). Tabla de usuarios (asociados a un rol)
4). Tabla de productos (drones)
5). Tabla de categorias de productos (categorias de drones)
6). Tabla de ventas
7). Tabla de rentas
8). Tabla de servicios
9). Tablas relacionadas al carrito y pedidos

El proyecto incluye scripts SQL para la creación completa de la base de datos, listos para ejecutarse sin ajustes adicionales.

## Interfaz de usuario

La interfaz está construida con MudBlazor, utilizando:

1). Layout principal con barra de navegación
2). Componentes responsivos
3). Mensajes visuales para estados de producto
4). Iconografía y estilos modernos

## Estructura del proyecto

1). Components: Componentes reutilizables, Layout, Pages: Páginas Blazor (Index, Details, Carrito, About, etc.)
2). Enums: Enumeraciones para estados del sistema
3). Data: Contexto de base de datos y entidades
4). Services: Lógica de negocio (carrito, productos, pedidos)

## Requisitos para ejecutar el proyecto

1). .NET SDK compatible con ASP.NET Core
2). SQL Server
3). Visual Studio o Visual Studio Code

## Configuración inicial

1. Clonar el repositorio.
2. Ejecutar el script SQL para crear la base de datos.
3. Configurar la cadena de conexión en `appsettings.json`.
4. Restaurar dependencias.
5. Ejecutar el proyecto.

## Objetivo del proyecto

El objetivo principal de Web Drones es servir como un sistema funcional de comercio electrónico básico, demostrando el uso de Blazor Server, manejo de estados, validaciones de negocio y una experiencia de usuario clara, sin depender de pasarelas de pago externas.

## Estado del proyecto

Proyecto funcional y en desarrollo continuo, con posibilidad de ampliar funcionalidades como autenticación avanzada, historial de pedidos y métodos de pago.

---

Desarrollado como parte del proyecto **Web Drones**.
