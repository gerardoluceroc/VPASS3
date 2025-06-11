import axios from "axios";
import { useState } from "react";
import { url_loginSession, url_logoutSession } from "../../services/API/API-VPASS3";
import { useDispatch } from "react-redux";
import { persistor } from "../../store/store";
import { disconnect, setUser } from "../../store/misSlice";
import { obtenerClaimsToken } from "../../utils/funciones";

const useLogin = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);  // Guardar la respuesta completa de la API
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

          // Decodificar el token para obtener los claims
          const claims = obtenerClaimsToken(token);
          
          const {
            exp: claimExpiracion, 
            establishment_id,
            ["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"]: email
          } = claims; // Desestructuración para obtener el timestamp de la fecha de expiración del token
          dispatch(setUser(
            {
              authenticated: true,
              token: token,
              rememberMe: true,
              idEstablishment: establishment_id,
              expirationTokenTimestamp: claimExpiracion, // Guardar la fecha de expiración en el estado
              email: email
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
          setLoading(false);  // Se indica que la petición terminó, independientemente de si tuvo éxito o no
      }
  };

  const logoutSession = async () => {
    setLoading(true);  // Indicamos que la petición está en proceso
    try{
      const response = await axios.post(url_logoutSession, {});

      // Limpiar manualmente las cabeceras de axios
      delete axios.defaults.headers.common['Authorization'];

      // Despacha la acción disconnect para limpiar el estado en Redux
      dispatch(disconnect());

      const handleReset = async () => {
        // Despacha la acción RESET para limpiar el estado en Redux
        dispatch({ type: 'RESET' });

        // Limpia los datos persistidos
        await persistor.purge();
      };
      handleReset();
    }
    catch (error) {
      const errorMessage = error?.response?.data?.message || "Error desconocido";
      const status = error?.response?.status || null;
      setResponse(errorMessage);
      setResponseStatus(status);
      return {status: status, message: errorMessage};
    } 
    finally {
      setLoading(false);  // Se indica que la petición terminó, independientemente de si tuvo éxito o no

    }
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