import { useState } from "react";
import { path_getAllSentidos } from "../../../services/API/API-VPASS3";
import axios from "axios";

const useSentido = () => {
  const [loading, setLoading] = useState(false);
  const [response, setResponse] = useState(null);
  const [sentidos, setSentidos] = useState(null);
  const [responseStatus, setResponseStatus] = useState(null);

  const getAllSentidos = async () => {
    setLoading(true);
    try {
      const response = await axios.get(path_getAllSentidos);
      const status = response?.status || null;
      setResponse(response || null);
      setResponseStatus(status);
      setSentidos(response?.data?.data || null);
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
    getAllSentidos,
    sentidos
  };
}

export default useSentido;
