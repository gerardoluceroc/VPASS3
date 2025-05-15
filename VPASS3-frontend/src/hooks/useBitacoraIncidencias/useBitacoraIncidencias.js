import axios from "axios";
import { path_getAllBitacoraIncidencias } from "../../services/API/API-VPASS3";
import { useState } from "react";

const useBitacoraIncidencias = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [bitacoraIncidencias, setBitacoraIncidencias] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
  
    const getAllBitacoraIncidencias = async () => {
      setLoading(true);
      try {
        const response = await axios.get(path_getAllBitacoraIncidencias);
        const status = response?.status || null;
        setResponse(response || null);
        setResponseStatus(status);
        setBitacoraIncidencias(response?.data?.data || null);
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
      getAllBitacoraIncidencias,
      bitacoraIncidencias,
    };
  }
  
  export default useBitacoraIncidencias;