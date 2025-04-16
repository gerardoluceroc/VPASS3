import axios from "axios";
import { useSelector } from "react-redux";

// Define una función que configura un interceptor para las solicitudes salientes de Axios
export function InterceptorRequest() {
    // Obtiene el estado del usuario desde Redux utilizando useSelector
    const data = useSelector(state => state.user);

    // Registra un interceptor que se ejecuta antes de que cada solicitud sea enviada
    axios.interceptors.request.use(
        (config) => {
            // Extrae el token del estado del usuario
            const token = data.token;

            // Si el token existe, lo agrega al encabezado Authorization de la solicitud
            if (token) {
                config.headers['Authorization'] = `Bearer ${token}`;
            }

            // Retorna la configuración modificada de la solicitud
            return config;
        },
        (error) => {
            // En caso de error al preparar la solicitud, rechaza la promesa con el error
            return Promise.reject(error);
        }
    );
}



export function InterceptorResponse(){
    axios.interceptors.response.use(
      response => {
          return response;
      },
      error => {
          // Caso 1: El cliente no tiene conexión a internet o el servidor no responde
          if (!error.response) {
              console.error("Error de red o sin conexión a internet: ", error.message);
              return Promise.reject("No se pudo conectar con el servidor. Verifique su conexión a internet.");
          }
  
          // Caso 2: La respuesta del servidor tiene un código de error (4xx, 5xx)
          // const status = error.response.status;
          // if (status >= 400 && status < 500) {
          //     console.error(`Error en la solicitud: Código ${status} - `, error.response.data);
          //     return Promise.reject({
          //         message: `Error del cliente: Código ${status} - ${error.response.data.message || "Error en la petición."}`
          //     });
          // }
  
          // // Caso 3: Error del servidor (5xx)
          // if (status >= 500) {
          //     console.error(`Error en el servidor: Código ${status} - `, error.response.data);
          //     return Promise.reject({
          //         message: `Error del servidor: Código ${status} - ${error.response.data.message || "Problema en el servidor."}`
          //     });
          // }
          // Rechazar el error si no cae en los casos anteriores
          return Promise.reject(error);
      }
    );
  }