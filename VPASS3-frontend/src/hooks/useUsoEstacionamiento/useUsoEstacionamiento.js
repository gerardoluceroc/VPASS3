import axios from "axios";
import { path_createUsoEstacionamiento, path_getAllUsoEstacionamiento } from "../../services/API/API-VPASS3";
import { useState } from "react";

const useUsoEstacionamiento = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);
    const [usoEstacionamiento, setUsoEstacionamiento] = useState();

    const registrarUsoEstacionamiento = async (idVisita) => {
        setLoading(true);
        try {
        const {data: responseRegistrarUsoEstacionamiento} = await axios.post(path_createUsoEstacionamiento, {
            idVisit: idVisita
        });

        // Primero se intenta crear una nueva zona
        const { data: registroUsoEstacionamientoCreado, statusCode: statusRegistrarUsoEstacionamiento, message: messageResgistrarUsoEstacionamiento } = responseRegistrarUsoEstacionamiento;
        setResponse(registroUsoEstacionamientoCreado);
        setResponseStatus(statusCrearZona);
        return responseRegistrarUsoEstacionamiento;

        } catch (error) {
            const errorMessage = error?.response?.data?.message || "Error desconocido";
            const status = error?.response?.status || null;
            setResponse(errorMessage);
            setResponseStatus(status);
            return error;
        } finally {
        setLoading(false);
        }
    };

    const getAllUsoEstacionamiento = async () => {
        setLoading(true);
        try {
            const { data: responseGetAllUsoEstacionamiento } = await axios.get(path_getAllUsoEstacionamiento);
            const { data: allUsoEstacionamiento, statusCode: statusGetAllUsoEstacionamiento, message: messageGetAllUsoEstacionamiento } = responseGetAllUsoEstacionamiento;
            setResponse(allUsoEstacionamiento);
            setResponseStatus(statusGetAllUsoEstacionamiento);
            setUsoEstacionamiento(allUsoEstacionamiento);
            return responseGetAllUsoEstacionamiento;
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
        registrarUsoEstacionamiento,
        getAllUsoEstacionamiento,
        usoEstacionamiento,
    };
};

export default useUsoEstacionamiento;