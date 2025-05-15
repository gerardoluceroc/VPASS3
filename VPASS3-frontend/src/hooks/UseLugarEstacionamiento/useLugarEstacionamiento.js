import axios from "axios";
import { path_getAllLugaresEstacionamiento } from "../../services/API/API-VPASS3";
import { useState } from "react";

const useLugaresEstacionamiento = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [lugaresEstacionamiento, setLugaresEstacionamiento] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
  
    const getAllLugaresEstacionamiento = async () => {
      setLoading(true);
      try {
        const response = await axios.get(path_getAllLugaresEstacionamiento);
        const status = response?.status || null;
        setResponse(response || null);
        setResponseStatus(status);
        setLugaresEstacionamiento(response?.data?.data || null);
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
      getAllLugaresEstacionamiento,
      lugaresEstacionamiento,
    };
  }
  
  export default useLugaresEstacionamiento;