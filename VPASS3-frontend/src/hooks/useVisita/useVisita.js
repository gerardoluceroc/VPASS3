import { useState } from "react";
import useVisitante from "../useVisitante/useVisitante";
import axios from "axios";
import { path_createVisita, path_getAllVisitas } from "../../services/API/API-VPASS3";

const useVisita = () => {

    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [visitas, setVisitas] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);

    const {crearVisitante, getVisitanteByIdentificationNumber} = useVisitante();

    const getAllVisitas = async () => {
        setLoading(true);
        try {
          const response = await axios.get(path_getAllVisitas);
          const status = response?.status || null;
          const responseData = response?.data || null;
          const visitasData = response?.data?.data || null;
          setResponse(responseData);
          setResponseStatus(status);
          setVisitas(visitasData);
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

    const crearVisita = async ({
      nombres,
      apellidos,
      numeroIdentificacion,
      idTipoVisita,
      idZona,
      idSubZona,
      idSentido,
      incluyeVehiculo,
      patenteVehiculo,
      idEstacionamiento
    }) => {
      // Indica visualmente que el proceso está en curso (puede mostrar un spinner, desactivar botones, etc.)
      setLoading(true);
    
      try {
        let idVisitante = null;
        console.log("creando visitante");
        const responseCrearVisitante = await crearVisitante({
          nombres,
          apellidos,
          numeroIdentificacion
        });
        console.log("visitante creado, responseCrearVisitante", responseCrearVisitante);

        // Primero se intenta crear un nuevo visitante
        const { data: visitanteCreado, statusCode: statusCrearVisitante, message: messageCrearVisitante } = responseCrearVisitante;

        if (statusCrearVisitante === 409) {
          // Si el visitante ya existe (código 409), se debe buscar por número de identificación
          const {data: visitanteExistente} = await getVisitanteByIdentificationNumber(numeroIdentificacion);
          console.log("visitanteExistente", visitanteExistente);

          // Se extrae el ID del visitante existente
          idVisitante = visitanteExistente?.id;
    
        } else if (statusCrearVisitante === 201 || statusCrearVisitante === 200) {
          // Si el visitante fue creado exitosamente, se obtiene el ID directamente de la respuesta
          idVisitante = visitanteCreado?.id;
    
        } else {
          // Si no se obtuvo un código 201 o 409, se considera un error inesperado
          throw responseCrearVisitante;
        }
        
        console.log("creando visita");
        // Una vez obtenido el ID del visitante (nuevo o existente), se procede a crear la visita
        const {data: respuestaCrearVisita} = await axios.post(path_createVisita, {
          visitorId: idVisitante,
          zoneId: idZona,
          idDirection: idSentido,
          idZoneSection: idSubZona,
          vehicleIncluded: incluyeVehiculo,
          licensePlate: incluyeVehiculo ? patenteVehiculo : "",
          idParkingSpot: incluyeVehiculo ? idEstacionamiento : null,
          idVisitType: idTipoVisita
        });

        console.log("visita creada, respuestaCrearVisita", respuestaCrearVisita);
        setResponse(respuestaCrearVisita);
        setResponseStatus(respuestaCrearVisita?.statusCode || null);
    
        // Si todo salió bien, se retorna la respuesta de la creación de la visita
        return respuestaCrearVisita;
    
      } catch (error) {
          console.log("error al crear visitante", error);
          const dataError = error?.response?.data || error || "Error desconocido";
          console.log("dataError", dataError);
          const status = error?.status ?? error?.statusCode ?? null;
          setResponse(dataError);
          setResponseStatus(status);
          return dataError;
      } finally {
          // Se indica que el proceso ha terminado, sea exitoso o con error
          setLoading(false);
      }
    };
     
    return {
        loading,
        response,
        responseStatus,
        visitas,
        crearVisita,
        getAllVisitas
    };
};
export default useVisita;