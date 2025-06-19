import axios from "axios";
import { path_getAllEspaciosComunes, path_getAllReservasEspaciosComunes, path_getAllUsosEspaciosComunes } from "../../services/API/API-VPASS3";
import { useState } from "react";

const useEspaciosComunes = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [espaciosComunes, setEspaciosComunes] = useState(null);
    const [reservasTotales, setReservasTotales] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
  
    const getAllEspaciosComunes = async () => {
        setLoading(true);
        try {
          const response = await axios.get(path_getAllEspaciosComunes);
          const status = response?.status || null;
          const responseData = response?.data || null;
          const espaciosComunesData = response?.data?.data || null;
          setResponse(responseData);
          setResponseStatus(status);
          setEspaciosComunes(espaciosComunesData);
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

    const getAllReservasEspaciosComunes = async () => {
        setLoading(true);
        try {
            const responseUsosEspaciosComunes = await axios.get(path_getAllUsosEspaciosComunes);
            const responseReservasEspaciosComunes = await axios.get(path_getAllReservasEspaciosComunes);

            const responseUsosEspaciosComunesData = responseUsosEspaciosComunes?.data || null;
            const responseReservasEspaciosComunesData = responseReservasEspaciosComunes?.data || null;

            const reservasEspaciosComunesData = responseReservasEspaciosComunesData?.data || null;
            const usosEspaciosComunesData = responseUsosEspaciosComunesData?.data || null;

            const reservasTotal = [...reservasEspaciosComunesData, ...usosEspaciosComunesData];

            const status = responseUsosEspaciosComunes?.status || null;
            setResponseStatus(status);
            setUsosEspaciosComunes(usosEspaciosComunesData);
            setReservasEspaciosComunes(reservasEspaciosComunesData);
            setReservasTotales(reservasTotal);
            return reservasTotal;
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

  
    return {
      loading,
      response,
      responseStatus,
      espaciosComunes,
      getAllEspaciosComunes,
      getAllReservasEspaciosComunes,
      reservasTotales,
    };
  }
  
  export default useEspaciosComunes;