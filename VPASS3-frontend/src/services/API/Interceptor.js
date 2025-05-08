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
    console.log("Ingresando al interceptor de respuesta");
    axios.interceptors.response.use(
      response => {
            return response; // Si la respuesta es exitosa, simplemente la retorna
      },
      error => {
          // Caso 1: El cliente no tiene conexión a internet o el servidor no responde
          if (!error.response) {
              console.error("Error de red o sin conexión a internet: ", error.message);
              return Promise.reject({statusCode: 0, data: null, message: "Error de red o sin conexión a internet"});
          }
          return Promise.reject(error);
      }
    );
}