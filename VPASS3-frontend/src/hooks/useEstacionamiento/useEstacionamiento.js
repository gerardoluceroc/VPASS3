import { useState } from "react";
import { path_getAllEstacionamientos, path_updateEstacionamiento } from "../../services/API/API-VPASS3";
import axios from "axios";

const useEstacionamiento = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [estacionamientos, setEstacionamientos] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
  
    const getAllEstacionamientos = async () => {
        setLoading(true);
        try {
          const response = await axios.get(path_getAllEstacionamientos);
          const status = response?.status || null;
          const responseData = response?.data || null;
          const estacionamientosData = response?.data?.data || null;
          setResponse(responseData);
          setResponseStatus(status);
          setEstacionamientos(estacionamientosData);
          return responseData;
        } catch (error) {
          const dataError = error?.response?.data || error || "Error desconocido";
          const status = error?.status ?? error?.statusCode ?? null;
          setResponse(dataError);
          setResponseStatus(status);
          return dataError;
        } finally {
          setLoading(false);
        }
    }

    const actualizarEstacionamiento = async (id, estacionamiento) => {
        setLoading(true);
        try {
            const response = await axios.put(path_updateEstacionamiento + "/" + id, estacionamiento);
            const status = response?.status || null;
            const responseData = response?.data || null;
            setResponse(responseData);
            setResponseStatus(status);
            return responseData;
          } catch (error) {
            const errorMessage = error?.response?.data?.message || "Error desconocido";
            const status = error?.response?.status || null;
            setResponse(errorMessage);
            setResponseStatus(status);
          } finally {
            setLoading(false);
          }
    }
  
  
  
  
  
    return {
      loading,
      response,
      responseStatus,
      getAllEstacionamientos,
      estacionamientos,
      actualizarEstacionamiento
    };
  }
  
  export default useEstacionamiento;