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
    * Además, se creará un usuario **superadministrador** por defecto, el cual tiene acceso a toda la información de todos los edificios y permisos para modificar la configuración del sistema. Las credenciales serán las siguientes:
       * **SUPERADMIN** Correo:  `superadmin@vpass3.cl` | Contraseña: `Clave123.`
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

## Despliegue (Producción o Pruebas)

Para desplegar y probar la aplicación en un entorno de producción o staging, es necesario utilizar Docker y Docker Compose. Esto permite ejecutar la aplicación en contenedores, garantizando un entorno consistente y reproducible.

Para obtener información más detallada o si encuentra algún problema durante la instalación, puede consultar la página oficial de Docker: [https://docs.docker.com/engine/install/ubuntu/](https://docs.docker.com/engine/install/ubuntu/)

### Requisitos Previos (Ubuntu o Windows con WSL)

Es necesario tener **Docker Engine** y **Docker Compose plugin** instalados en su servidor o máquina local con Ubuntu/WSL.

#### **Instalación de Docker y Docker Compose en Ubuntu (Usando el Repositorio APT)**

Se recomienda seguir las instrucciones oficiales de la documentación de Docker para una instalación completa y segura. Los siguientes pasos resumen el proceso para Ubuntu utilizando el repositorio `apt`.

1.  **Configurar el Repositorio APT de Docker:**
    * Actualice el índice de paquetes e instale los paquetes necesarios para permitir que `apt` use un repositorio a través de HTTPS:
        ```bash
        sudo apt-get update
        sudo apt-get install ca-certificates curl
        ```
    * Cree el directorio clave para Docker y descargue la clave GPG oficial de Docker:
        ```bash
        sudo install -m 0755 -d /etc/apt/keyrings
        sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
        sudo chmod a+r /etc/apt/keyrings/docker.asc
        ```
    * Agregue el repositorio de Docker a las fuentes de `apt`:
        ```bash
        echo \
          "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] [https://download.docker.com/linux/ubuntu](https://download.docker.com/linux/ubuntu) \
          $(. /etc/os-release && echo "${UBUNTU_CODENAME:-$VERSION_CODENAME}") stable" | \
          sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
        ```
    * Actualice el índice de paquetes de `apt` nuevamente para incluir el nuevo repositorio:
        ```bash
        sudo apt-get update
        ```

2.  **Instalar los Paquetes de Docker:**
    * Instale Docker Engine, Docker CLI, containerd y los plugins de Docker Buildx y Docker Compose:
        ```bash
        sudo apt-get install docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
        ```

3.  **Verificar la Instalación (Opcional):**
    * Para verificar que Docker Engine se haya instalado correctamente y que pueda ejecutar contenedores, ejecute la imagen `hello-world`:
        ```bash
        sudo docker run hello-world
        ```
    * Este comando descarga una imagen de prueba y la ejecuta en un contenedor. Debería ver un mensaje de confirmación en su terminal.

  ### Despliegue de la Aplicación con Docker Compose

Para desplegar la aplicación, es fundamental configurar el archivo `.env` de Docker Compose según el entorno de destino. Este archivo se encuentra en la raíz del repositorio, junto al `docker-compose.yml`.

A continuación, se detallan las opciones de configuración para la sección relevante del archivo `.env`:

```dotenv
# --- DISTINTAS OPCIONES DE CONFIGURACION (DESCOMENTAR LINEAS SEGUN EL CASO, EL RESTO SE DEBE DEJAR COMENTADO) --- #
# Se asume que el backend escucha en el puerto 5113 y el frontend en el puerto 5173.

# CONFIGURACION PARA INSTALAR EN MAQUINA LOCAL WINDOWS CON DOCKER DESKTOP O UBUNTU
# URL_API_BACKEND=http://localhost:5113
# CORS_ALLOWED_ORIGINS_LIST=http://localhost:5173

# CONFIGURACION PARA INSTALAR EN SERVIDOR REMOTO UBUNTU SIN DOMINIO Y SIN NGINX (ip publica ejemplo 143.198.64.45)
# URL_API_BACKEND=http://143.198.64.45:5113
# CORS_ALLOWED_ORIGINS_LIST=http://143.198.64.45:5173

# CONFIGURACION PARA INSTALAR EN SERVIDOR REMOTO UBUNTU CON DOMINIO (HTTPS) Y SIN NGINX (dominio ejemplo dominioejemplo.com)
# URL_API_BACKEND=https://dominioejemplo.com:5113
# CORS_ALLOWED_ORIGINS_LIST=https://dominioejemplo.com:5173
```
**Pasos para configurar el archivo `.env`:**

1.  **Abrir el archivo:** Se debe navegar hasta la raíz de su repositorio y abrir el archivo `.env`.
2.  **Seleccionar la configuración:**
    * **Caso 1: Despliegue en máquina local (Windows con Docker Desktop o Ubuntu):**
        * Se deben descomentar las siguientes líneas y asegurarse de que el resto de las opciones de `URL_API_BACKEND` y `CORS_ALLOWED_ORIGINS_LIST` queden comentadas:
            ```dotenv
            URL_API_BACKEND=http://localhost:5113
            CORS_ALLOWED_ORIGINS_LIST=http://localhost:5173
            ```
    * **Caso 2: Despliegue en servidor remoto Ubuntu sin dominio y sin Nginx (usando IP pública):**
        * Se deben descomentar las siguientes líneas y asegurarse de que el resto de las opciones de `URL_API_BACKEND` y `CORS_ALLOWED_ORIGINS_LIST` queden comentadas:
            ```dotenv
            URL_API_BACKEND=http://143.198.64.45:5113
            CORS_ALLOWED_ORIGINS_LIST=http://143.198.64.45:5173
            ```
        * **Importante:** Se debe reemplazar `143.198.64.45` con la dirección IP pública real de su servidor.
    * **Caso 3: Despliegue en servidor remoto Ubuntu con dominio (HTTPS) y sin Nginx:**
        * Se deben descomentar las siguientes líneas y asegurarse de que el resto de las opciones de `URL_API_BACKEND` y `CORS_ALLOWED_ORIGINS_LIST` queden comentadas:
            ```dotenv
            URL_API_BACKEND=https://dominioejemplo.com:5113
            CORS_ALLOWED_ORIGINS_LIST=https://dominioejemplo.com:5173
            ```
        * **Importante:** Se debe reemplazar `dominioejemplo.com` con el nombre de dominio real asociado a su servidor.

3.  **Guardar cambios:** Se debe guardar el archivo `.env` después de realizar las modificaciones.

Una vez configurado el archivo `.env` para el entorno deseado, el despliegue de la aplicación se realiza con un único comando de Docker Compose.

### Ejecución de la Aplicación con Docker Compose

1.  **Navegar al Directorio:** Se debe abrir una terminal (ej. SSH al servidor remoto) y navegar hasta la raíz del repositorio de su proyecto, donde se encuentran el `docker-compose.yml` y el archivo `.env`.
2.  **Iniciar los Servicios:** Se debe ejecutar el siguiente comando para construir las imágenes y levantar todos los servicios definidos en el `docker-compose.yml`:
    ```bash
    docker compose up --build -d
    ```
    * `--build`: Este flag asegura que las imágenes de los servicios (frontend y backend) se reconstruyan. Esto es crucial si se han realizado cambios en el código de la aplicación o en los `Dockerfile`s.
    * `-d`: Este flag ejecuta los contenedores en modo "detached" (en segundo plano), liberando la terminal.

3.  **Verificar el Estado de los Contenedores (Opcional):**
    * Para verificar que todos los contenedores estén corriendo correctamente, se puede usar:
        ```bash
        docker compose ps
        ```

4.  **Acceso a la Aplicación:**
    * Una vez que los servicios estén en funcionamiento, se debe abrir un navegador web e ingresar la dirección IP pública o el dominio de su servidor, seguido del puerto `5173` (si se está usando HTTP sin Nginx/proxy inverso, como en los ejemplos del `.env`).
    * Por ejemplo, para el caso de una instalacion en maquina local sería: `http://localhost:5173`
    * Si se usó la IP pública de ejemplo: `http://143.198.64.45:5173`.
    * Si se usó el dominio de ejemplo: `https://dominioejemplo.com:5173`
    * Esto cargará la vista de login de la aplicación, permitiendo al usuario iniciar sesión y probar el sistema.
