import axios from "axios";
import { useState } from "react";
import { url_loginSession } from "../../services/API/API-VPASS3";

const useLogin = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);  // Guardar la respuesta completa de la API
    // const [error, setError] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
  
    const loginSession = async ({email, password}) => {
      const body = {
        email,
        password,
      };
  
      setLoading(true);  // Indicamos que la petición está en proceso
      try {
        const response = await axios.post(url_loginSession, body);  // Realizas la petición POST      
        const token = response?.data?.data?.token || null;  // Asegurarse que token no sea undefined
        const status = response?.status || null;  // Asegurarse que status no sea undefined
  
        if (token) {
          setResponse(token);
          setResponseStatus(status);
          return { token, status }; 
        } else {
          throw new Error("Token no disponible");
        }
  
      } catch (error) {
          const errorMessage = error?.response?.data?.message || "Error desconocido";  // Manejo de errores
          const status = error?.response?.data?.statusCode || null;
          setResponse(errorMessage);
          setResponseStatus(status);
          return {token: null, status: status};
      } finally {
          setLoading(false);  // Indicamos que la petición terminó, independientemente de si tuvo éxito o no
      }
  };
  
  
    return {
      loading,
      response,
      loginSession,
      responseStatus
    };
  };
  
  export default useLogin;
  