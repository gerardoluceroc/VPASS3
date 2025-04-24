import { useState } from "react";
import { path_getAllTiposVisita } from "../../services/API/API-VPASS3";
import axios from "axios";

const useTiposVisita = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [tiposVisita, setTiposVisita] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
  
    const getAllTiposVisita = async () => {
      setLoading(true);
      try {
        const response = await axios.get(path_getAllTiposVisita);
        const status = response?.status || null;
        setResponse(response || null);
        setResponseStatus(status);
        setTiposVisita(response?.data?.data || null);
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
      getAllTiposVisita,
      tiposVisita,
    };
  }
  
  export default useTiposVisita;