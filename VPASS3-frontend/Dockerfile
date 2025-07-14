# Etapa 1: Construir la aplicación React
# Usamos una imagen de Node.js que coincide con tu versión de desarrollo (20.14.0)
FROM node:20.14.0-alpine AS builder

# Establece el directorio de trabajo dentro del contenedor
WORKDIR /app

# Copia package.json y package-lock.json
COPY package.json ./
# Si tienes un package-lock.json, puedes descomentar la siguiente línea:
# COPY package-lock.json ./

# Instala TODAS las dependencias (incluidas las de desarrollo), ya que Vite es necesario para la construcción.
# Añadimos --legacy-peer-deps para resolver el conflicto de dependencias.
RUN npm install --legacy-peer-deps

# Copia el resto del código de tu aplicación
COPY . .

# Construye la aplicación React para producción
# Este comando creará la carpeta 'dist' con la versión optimizada de tu app
RUN npm run build

# Etapa 2: Servir la aplicación construida
# Reutilizamos la misma imagen de Node.js para servir la aplicación
FROM node:20.14.0-alpine

# Establece el directorio de trabajo
WORKDIR /app

# Copia TODO el contenido del directorio /app de la etapa 'builder' a la etapa final.
# Esto incluye package.json, node_modules y la carpeta dist,
# asegurando que 'npm run preview' tenga todo lo que necesita.
COPY --from=builder /app .

# Expone el puerto 4173, que es el puerto por defecto de Vite para 'preview'
EXPOSE 4173

# ¡IMPORTANTE CAMBIO AQUÍ!
# Comando para iniciar la aplicación usando 'npm run preview' y que escuche en todas las interfaces.
CMD ["npm", "run", "preview", "--", "--host"]