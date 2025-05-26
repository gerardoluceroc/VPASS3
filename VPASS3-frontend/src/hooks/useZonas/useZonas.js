import { useState } from "react";
import { path_deleteZona, path_getAllZonas } from "../../services/API/API-VPASS3";
import axios from "axios";

const useZonas = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [zonas, setZonas] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
  
    const getAllZonas = async () => {
      setLoading(true);
      try {
        const response = await axios.get(path_getAllZonas);
        const status = response?.status || null;
        setResponse(response || null);
        setResponseStatus(status);
        setZonas(response?.data?.data || null);
      } catch (error) {
        const errorMessage = error?.response?.data?.message || "Error desconocido";
        const status = error?.response?.status || null;
        setResponse(errorMessage);
        setResponseStatus(status);
      } finally {
        setLoading(false);
      }
    }

    const eliminarZona = async (id) => {
        setLoading(true);
        try {
            const response = await axios.delete(path_deleteZona + "/" + id);
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
            return error;
          } finally {
            setLoading(false);
          }
    }

    return {
      loading,
      response,
      responseStatus,
      getAllZonas,
      zonas,
      eliminarZona
    };
  }
  
  export default useZonas;