import { useState } from 'react'
import usePersona from '../usePersona/usePersona';
import axios from 'axios';
import { path_createReservaExclusivaEspacioComun, path_createReservaUsoCompartido } from '../../services/API/API-VPASS3';

const useReservarEspacioComun = () => {
    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [reservacionesTipoReservaExclusiva, setReservacionesTipoReservaExclusiva] = useState(null);
    const [reservacionesTipoUsoCompartido, setReservacionesTipoUsoCompartido] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);

    const {getPersonaByIdentificationNumber, crearPersona} = usePersona();

    const crearReservaExclusivaEspacioComun = async (
        nombres, 
        apellidos, 
        numeroIdentificacion, 
        idAreaComun, 
        fechaReserva, 
        duracionReserva, 
        cantidadInvitados
    ) => {
        setLoading(true);
        try {
            let personaReserva;
            let respuestaObtenerPersona = await getPersonaByIdentificationNumber(numeroIdentificacion);

            const {data: dataObtenerPersona, statusCode: statusCodeObtenerPersona, message: messageObtenerPersona} = respuestaObtenerPersona;

            if(statusCodeObtenerPersona === 404){
                const {data: dataCrearPersona} = await crearPersona({
                    nombres,
                    apellidos,
                    numeroIdentificacion
                })
                personaReserva = dataCrearPersona;
            }

            else if(statusCodeObtenerPersona === 200){
                personaReserva = dataObtenerPersona;
            }
            else{
                // Si no se obtuvo un código 200 o 404, se considera un error inesperado
                throw respuestaObtenerPersona;
            }

            const cuerpoPeticion = {
                idCommonArea: idAreaComun,
                reservationStart: fechaReserva,
                reservationTime: duracionReserva,
                idPersonReservedBy: personaReserva.id,
                guestsNumber: cantidadInvitados
            }

            const {data: responseCrearReservaExclusivaEspacioComun} = await axios.post(path_createReservaExclusivaEspacioComun, cuerpoPeticion);

            const {data: reservaCreada, statusCode: statusCrearReserva, message: messageCrearReserva} = responseCrearReservaExclusivaEspacioComun;
            setResponse(reservaCreada);
            setResponseStatus(statusCrearReserva);
            return responseCrearReservaExclusivaEspacioComun;

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

    const crearReservaUsoCompartido = async (
        nombres, 
        apellidos, 
        numeroIdentificacion, 
        idAreaComun, 
        fechaReserva, 
        duracionReserva, 
        cantidadInvitados
    ) => {
        setLoading(true);
        try {
            let personaReserva;
            let respuestaObtenerPersona = await getPersonaByIdentificationNumber(numeroIdentificacion);

            const {data: dataObtenerPersona, statusCode: statusCodeObtenerPersona, message: messageObtenerPersona} = respuestaObtenerPersona;

            if(statusCodeObtenerPersona === 404){
                const {data: dataCrearPersona} = await crearPersona({
                    nombres,
                    apellidos,
                    numeroIdentificacion
                })
                personaReserva = dataCrearPersona;
            }

            else if(statusCodeObtenerPersona === 200){
                personaReserva = dataObtenerPersona;
            }
            else{
                // Si no se obtuvo un código 200 o 404, se considera un error inesperado
                throw respuestaObtenerPersona;
            }

            const cuerpoPeticion = {
                idCommonArea: idAreaComun,
                idPerson: personaReserva.id,
                startTime: fechaReserva,
                usageTime: duracionReserva,
                guestsNumber: cantidadInvitados
            }

            const {data: responseCrearReservaUsoCompartido} = await axios.post(path_createReservaUsoCompartido, cuerpoPeticion);
            const {data: reservaCreada, statusCode: statusCrearReserva, message: messageCrearReserva} = responseCrearReservaUsoCompartido;
            setResponse(reservaCreada);
            setResponseStatus(statusCrearReserva);
            return responseCrearReservaUsoCompartido;

        } catch (error) {
            const errorMessage = error?.response?.data?.message || "Error desconocido";
            const status = error?.response?.status || null;
            setResponse(errorMessage);
            setResponseStatus(status);
            return error?.response?.data || error;
        } finally {
            setLoading(false);
        }
        
    }
  return {
    loading,
    response,
    responseStatus,
    reservacionesTipoReservaExclusiva,
    reservacionesTipoUsoCompartido,
    crearReservaExclusivaEspacioComun,
    crearReservaUsoCompartido,
  }
}

export default useReservarEspacioComun;