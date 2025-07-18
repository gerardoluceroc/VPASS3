# Sistema de Control de Visitas

Este software tiene como objetivo gestionar las visitas que recibe un edificio. Además, se pueden gestionar otro tipo de cosas como la llegada de encomiendas, reservas de espacios comunes, inquilinos viviendo en los departamentos, uso de estacionamientos, etc.

## Instrucciones de Uso (Modo Desarrollo)

Para desarrollar y ejecutar este proyecto en un entorno local (especialmente en Windows), se deben cumplir los siguientes requisitos y seguir los pasos detallados a continuación.

---

### Requisitos Previos (Windows)

Asegúrate de tener instaladas las siguientes herramientas en tu sistema Windows:

* **.NET 8 SDK:** Es el kit de desarrollo de software para aplicaciones .NET.
    * [Descargar .NET 8 SDK](https://dotnet.microsoft.com/es-es/download/dotnet/8.0)
* **SQL Server 2022:** Sistema de gestión de bases de datos relacionales.
    * [Descargar SQL Server 2022 Express Edition](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (para desarrollo local)
* **SQL Server Management Studio (SSMS) 2022:** Herramienta gráfica para gestionar y administrar SQL Server.
    * [Descargar SQL Server Management Studio (SSMS)](https://docs.microsoft.com/es-es/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16)
* **Node.js (versión 20.14.0 o cercana):** Entorno de ejecución de JavaScript necesario para el frontend de React.
    * [Descargar Node.js](https://nodejs.org/es/download/)

---

### Ejecución del Backend (.NET)

1.  **Abrir el Proyecto:** Navega hasta la carpeta `VPASS3-backend` y abre el archivo de solución `VPASS3-backend.sln` con Visual Studio.
2.  **Iniciar la Aplicación:** Una vez abierto el proyecto en Visual Studio, presiona el botón "Play" (normalmente un triángulo verde con "IIS Express" o "http") en la barra de herramientas.
3.  **Inicialización de Datos:** Al iniciar el backend por primera vez, se ejecutarán procesos de siembra de datos (seeding):
    * Se cargarán datos iniciales esenciales para el funcionamiento de la aplicación (roles de usuario, configuraciones básicas).
    * Se creará un edificio de ejemplo.
    * Se crearán dos usuarios administradores de ejemplo para el edificio:
        * **Admin 1:** Correo: `admin1@edificiotest.com` | Contraseña: `Clave123.`
        * **Admin 2:** Correo: `admin2@edificiotest.com` | Contraseña: `Clave321.`
    * **Nota Importante:** Estos administradores solo tienen acceso a la información y permisos de su propio edificio. No pueden ver ni modificar datos de otros edificios.
    * Además, se creará un usuario **superadministrador** por defecto, el cual tiene acceso a toda la información de todos los edificios y permisos para modificar la configuración del sistema.
4.  **Acceso a la API:** Una vez que el backend se ejecuta, se desplegará automáticamente en tu navegador la interfaz de **Swagger UI**. Esta interfaz proporciona la documentación de todos los controladores de la API y permite realizar pruebas de las peticiones directamente desde el navegador.
5.  **Puerto del Backend:** El backend se ejecutará y escuchará peticiones en el puerto `5113` (ej. `http://localhost:5113`).

---

### Ejecución del Frontend (React con Vite)

1.  **Navegar al Directorio:** Abre una terminal (ej. PowerShell, CMD o Git Bash) y navega hasta la carpeta de tu proyecto de frontend: `cd VPASS3-frontend`.
2.  **Instalar Dependencias:** Ejecuta el siguiente comando para instalar todas las dependencias de Node.js necesarias para el proyecto:
    ```bash
    npm install
    ```
3.  **Iniciar la Aplicación:** Una vez instaladas las dependencias, inicia el servidor de desarrollo de Vite con:
    ```bash
    npm run dev
    ```
4.  **Acceso al Frontend:** El frontend se ejecutará y estará accesible en el puerto `5173` (ej. `http://localhost:5173`) en tu navegador.