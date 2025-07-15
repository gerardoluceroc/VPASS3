# Etapa 1: Construcción de la aplicación React
# Se utiliza una imagen de Node.js que coincide con la versión de desarrollo (20.14.0).
FROM node:20.14.0-alpine AS builder

# Se establece el directorio de trabajo dentro del contenedor.
WORKDIR /app

# Se copian package.json y package-lock.json.
# Esto permite que Docker cachee la capa de instalación de dependencias.
COPY package.json ./
# Si existe un package-lock.json, se puede descomentar la siguiente línea:
# COPY package-lock.json ./

# Se instalan TODAS las dependencias (incluidas las de desarrollo), ya que Vite es necesario para la construcción.
# Se añade --legacy-peer-deps para resolver posibles conflictos de dependencias.
RUN npm install --legacy-peer-deps

# --- INICIO DE LAS LÍNEAS AÑADIDAS/MODIFICADAS ---
# Se declara un argumento de construcción (build arg) para recibir la URL de la API.
# Este argumento será proporcionado por Docker Compose durante el proceso de 'docker build'.
ARG VITE_APP_API_URL

# Se establece esta variable de entorno para que Vite la utilice durante el proceso de 'npm run build'.
# Vite necesita que la variable esté disponible en el entorno de construcción para incrustarla en el código JavaScript final.
ENV VITE_APP_API_URL=$VITE_APP_API_URL
# --- FIN DE LAS LÍNEAS AÑADIDAS/MODIFICADAS ---

# Se copia el resto del código de la aplicación.
COPY . .

# Se construye la aplicación React para producción.
# Este comando ahora tendrá acceso a VITE_APP_API_URL para compilar la URL del backend.
RUN npm run build

# Etapa 2: Servir la aplicación construida
# Se reutiliza la misma imagen de Node.js para servir la aplicación.
FROM node:20.14.0-alpine

# Se establece el directorio de trabajo.
WORKDIR /app

# Se copia TODO el contenido del directorio /app de la etapa 'builder' a la etapa final.
# Esto incluye package.json, node_modules y la carpeta dist,
# asegurando que 'npm run preview' tenga todo lo que necesita para ejecutarse.
COPY --from=builder /app .

# Se expone el puerto 4173, que es el puerto por defecto de Vite para 'preview'.
EXPOSE 4173

# Se define el comando para iniciar la aplicación utilizando 'npm run preview'.
# Se incluye '-- --host' para que la aplicación escuche en todas las interfaces,
# permitiendo la accesibilidad desde el host de Docker.
CMD ["npm", "run", "preview", "--", "--host"]