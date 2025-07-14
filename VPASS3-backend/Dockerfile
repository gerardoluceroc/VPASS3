# Etapa 1: Construcción de la aplicación
# Se utiliza la imagen del SDK de .NET 8 para compilar la aplicación.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Se establece el directorio de trabajo dentro del contenedor.
# Aquí es donde se copiarán los archivos del proyecto.
WORKDIR /src

# Se copia el archivo .csproj primero para permitir que Docker cachee la restauración de dependencias.
# El nombre del archivo de proyecto es VPASS3-backend.csproj
COPY ["VPASS3-backend.csproj", "./"]

# Se restauran las dependencias de NuGet.
# Esto descargará todos los paquetes necesarios definidos en el .csproj.
RUN dotnet restore

# Se copia el resto de los archivos del proyecto al directorio de trabajo.
COPY . .

# Se publica la aplicación. Esto compila la aplicación y la prepara para el despliegue,
# colocando los archivos de salida en el directorio /app/publish.
# Se utiliza el nombre exacto del proyecto: VPASS3-backend.csproj
RUN dotnet publish "VPASS3-backend.csproj" -c Release -o /app/publish --no-restore

# Etapa 2: Ejecución de la aplicación
# Se utiliza la imagen del runtime de ASP.NET Core 8.0, que es más pequeña y segura para producción.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Se establece el directorio de trabajo para la aplicación final.
WORKDIR /app

# Se copian los archivos publicados desde la etapa 'build'.
COPY --from=build /app/publish .

# Se expone el puerto 5113, que es el puerto HTTP que la aplicación .NET utiliza.
EXPOSE 5113

# Se configura la variable de entorno ASPNETCORE_URLS para que la aplicación escuche en todas las interfaces.
# Esto es crucial para que el contenedor sea accesible desde el host de Docker.
ENV ASPNETCORE_URLS=http://+:5113

# Se define el punto de entrada para ejecutar la aplicación.
# El nombre de la DLL principal es VPASS3-backend.dll
ENTRYPOINT ["dotnet", "VPASS3-backend.dll"]