import { useState } from "react";
import { path_deleteSubZona } from "../../services/API/API-VPASS3";
import axios from "axios";

const useSubZona = () => {

    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
    
    const eliminarSubZona = async (id) => {
        setLoading(true);
        try {
            const response = await axios.delete(path_deleteSubZona + "/" + id);
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
        eliminarSubZona,
    };
};
export default useSubZona;