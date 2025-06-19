import axios from "axios";
import { path_createPersona, path_getAllPersonas, path_getPersonaByIdentificationNumber } from "../../services/API/API-VPASS3";
import { useState } from "react";

const usePersona = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [personas, setPersonas] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);

    const getAllPersonas = async () => {
        setLoading(true);
        try {
            const response = await axios.get(path_getAllPersonas); // Cambia la URL si también renombraste los endpoints
            const status = response?.status || null;
            const responseData = response?.data || null;
            const personasData = response?.data?.data || null;
            setResponse(responseData);
            setResponseStatus(status);
            setPersonas(personasData);
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
    };

    const getPersonaByIdentificationNumber = async (identificationNumber) => {
        setLoading(true);
        try {
            const response = await axios.get(path_getPersonaByIdentificationNumber + identificationNumber);
            const status = response?.status || null;
            const responseData = response?.data || null;
            setResponse(responseData);
            setResponseStatus(status);
            setPersonas(responseData);
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
    };

    const crearPersona = async ({ nombres, apellidos, numeroIdentificacion }) => {
        const data = {
            names: nombres,
            lastnames: apellidos,
            identificationNumber: numeroIdentificacion
        };
        setLoading(true);
        try {
            const response = await axios.post(path_createPersona, data);
            const status = response?.status || null;
            const responseData = response?.data || null;
            const personaCreada = response?.data?.data || null;
            setResponse(responseData);
            setResponseStatus(status);
            setPersonas(personaCreada);
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
    };

    return {
        loading,
        response,
        responseStatus,
        personas,
        crearPersona,
        getAllPersonas,
        getPersonaByIdentificationNumber
    };
};

export default usePersona;
