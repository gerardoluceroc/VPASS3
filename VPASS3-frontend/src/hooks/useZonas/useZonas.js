import { useState } from "react";
import { path_getAllZonas } from "../../services/API/API-VPASS3";
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
  
  
  
  
  
    return {
      loading,
      response,
      responseStatus,
      getAllZonas,
      zonas,
    };
  }
  
  export default useZonas;