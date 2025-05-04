import axios from "axios";
import { useState } from "react";
import { url_loginSession } from "../../services/API/API-VPASS3";
import { useDispatch } from "react-redux";
import { persistor } from "../../store/store";
import { disconnect, setUser } from "../../store/misSlice";

const useLogin = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);  // Guardar la respuesta completa de la API
    // const [error, setError] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
    const dispatch = useDispatch();
  
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

          dispatch(setUser(
            {
              authenticated: true,
              token: token,
              rememberMe: true,
            }
          ))

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

  const logoutSession = () => {
    dispatch(disconnect(
      {
        authenticated: false,
        token: null,
        rememberMe: false,
      }
    ));

    const handleReset = async () => {
      // Despacha la acción RESET para limpiar el estado en Redux
      dispatch({ type: 'RESET' });

      // Limpia los datos persistidos
      await persistor.purge();
    };
    handleReset();
  }
  
  
    return {
      loading,
      response,
      loginSession,
      logoutSession,
      responseStatus
    };
  };
  
  export default useLogin;
  