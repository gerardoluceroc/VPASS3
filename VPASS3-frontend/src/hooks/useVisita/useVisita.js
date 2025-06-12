import { useState } from "react";
import useVisitante from "../useVisitante/useVisitante";
import axios from "axios";
import { path_createVisita, path_getAllVisitas, path_getReportePorRangoDeFechas, path_getReportePorRut } from "../../services/API/API-VPASS3";
import { cambiarAFormatoHoraMinutos } from "../../utils/funciones";
import { idSentidoVisitaEntrada } from "../../utils/constantes";
import useUsoEstacionamiento from "../useUsoEstacionamiento/useUsoEstacionamiento";

const useVisita = () => {

    const [loading, setLoading] = useState(false);
    const [response, setResponse] = useState(null);
    const [visitas, setVisitas] = useState(null);
    const [responseStatus, setResponseStatus] = useState(null);

    const {registrarUsoEstacionamiento} = useUsoEstacionamiento();
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
        idEstacionamiento,
        horasUsoEstacionamiento,
        minutosUsoEstacionamiento
    }) => {
      // Indica visualmente que el proceso está en curso (puede mostrar un spinner, desactivar botones, etc.)
      setLoading(true);
    
      try {
        let idVisitante = null;
        const responseCrearVisitante = await crearVisitante({
          nombres,
          apellidos,
          numeroIdentificacion
        });
        // Primero se intenta crear un nuevo visitante
        const { data: visitanteCreado, statusCode: statusCrearVisitante, message: messageCrearVisitante } = responseCrearVisitante;

        if (statusCrearVisitante === 409) {
          // Si el visitante ya existe (código 409), se debe buscar por número de identificación
          const {data: visitanteExistente} = await getVisitanteByIdentificationNumber(numeroIdentificacion);

          // Se extrae el ID del visitante existente
          idVisitante = visitanteExistente?.id;
    
        } else if (statusCrearVisitante === 201 || statusCrearVisitante === 200) {
          // Si el visitante fue creado exitosamente, se obtiene el ID directamente de la respuesta
          idVisitante = visitanteCreado?.id;
    
        } else {

          // Si no se obtuvo un código 201 o 409, se considera un error inesperado
          throw responseCrearVisitante;
        }
        
        // Una vez obtenido el ID del visitante (nuevo o existente), se procede a crear la visita
        const {data: respuestaCrearVisita} = await axios.post(path_createVisita, {
          visitorId: idVisitante,
          zoneId: idZona,
          idDirection: idSentido,
          idZoneSection: idSubZona,
          vehicleIncluded: incluyeVehiculo,
          licensePlate: incluyeVehiculo ? patenteVehiculo : "",
          idParkingSpot: incluyeVehiculo ? idEstacionamiento : null,
          idVisitType: idTipoVisita,
          authorizedTime: idSentido === idSentidoVisitaEntrada && incluyeVehiculo ? cambiarAFormatoHoraMinutos(horasUsoEstacionamiento, minutosUsoEstacionamiento) : null
        });

        setResponse(respuestaCrearVisita);
        setResponseStatus(respuestaCrearVisita?.statusCode || null);

        // Si la visita fue creada exitosamente e incluye vehiculo, se registra el uso del estacionamiento
        if(incluyeVehiculo) {

            const {data: visitaCreada} = respuestaCrearVisita;
            const respuestaRegistrarUsoEstacionamiento = await registrarUsoEstacionamiento(visitaCreada?.id);
        }
    
        // Si todo salió bien, se retorna la respuesta de la creación de la visita
        return respuestaCrearVisita;
    
      } catch (error) {
            console.log("error => ", error);
          const dataError = error?.response?.data || error || "Error desconocido";
          const status = error?.status ?? error?.statusCode ?? null;
          setResponse(dataError);
          setResponseStatus(status);
          return dataError;
      } finally {
          // Se indica que el proceso ha terminado, sea exitoso o con error
          setLoading(false);
      }
    };

    const getVisitasPorRangoDeFechas = async (fechaInicio, fechaFinal) => {
    setLoading(true);
    try {
        // Crear objeto con las fechas
        const dto = {
            StartDate: fechaInicio,
            EndDate: fechaFinal
        };

        // Hacer la petición para exportar a Excel
        const response = await axios.post(path_getReportePorRangoDeFechas,
            dto,
            {
                responseType: 'blob' // Para manejar la respuesta como archivo
            }
        );

        // Crear URL del blob
        const url = window.URL.createObjectURL(new Blob([response.data]));
        
        // Extraer nombre del archivo del backend
        const contentDisposition = response.headers['content-disposition'];
        let fileName = 'visitas.xlsx';
        
        if (contentDisposition) {
            const fileNameMatch = contentDisposition.match(/filename="?(.+)"?/);
            if (fileNameMatch && fileNameMatch[1]) {
                fileName = fileNameMatch[1];
            }
        }

        // Crear enlace y descargar
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
        
        // Limpiar
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);

        setResponse({ success: true });
        setResponseStatus(response.status);
        
        return { success: true, fileName };
    } catch (error) {
        let errorData;
        let status;
        
        if (error.response) {
            // Manejar error como blob si es necesario
            if (error.response.data instanceof Blob) {
                errorData = await error.response.data.text();
                try {
                    errorData = JSON.parse(errorData);
                } catch {
                    errorData = { message: errorData };
                }
            } else {
                errorData = error.response.data;
            }
            status = error.response.status;
        } else {
            errorData = error.message || 'Error desconocido';
            status = null;
        }

        setResponse(errorData);
        setResponseStatus(status);
        return { success: false, error: errorData };
    } finally {
        setLoading(false);
    }
};

const getReporteVisitasPorRut = async (rut) => {
    setLoading(true);
    try {
        const dto = {
            identificationNumber: rut
        };

        // Hacer la petición para exportar a Excel
        const response = await axios.post(path_getReportePorRut,
            dto,
            {
                responseType: 'blob' // Para manejar la respuesta como archivo
            }
        );

        // Crear URL del blob
        const url = window.URL.createObjectURL(new Blob([response.data]));
        
        // Extraer nombre del archivo del backend
        const contentDisposition = response.headers['content-disposition'];
        let fileName = `visitas_${rut}.xlsx`;
        
        if (contentDisposition) {
            const fileNameMatch = contentDisposition.match(/filename="?(.+)"?/);
            if (fileNameMatch && fileNameMatch[1]) {
                fileName = fileNameMatch[1];
            }
        }

        // Crear enlace y descargar
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        link.click();
        
        // Limpiar
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);

        setResponse({ success: true });
        setResponseStatus(response.status);
        
        return { success: true, fileName };
    } catch (error) {
        let errorData;
        let status;
        
        if (error.response) {
            // Manejar error como blob si es necesario
            if (error.response.data instanceof Blob) {
                errorData = await error.response.data.text();
                try {
                    errorData = JSON.parse(errorData);
                } catch {
                    errorData = { message: errorData };
                }
            } else {
                errorData = error.response.data;
            }
            status = error.response.status;
        } else {
            errorData = error.message || 'Error desconocido';
            status = null;
        }

        setResponse(errorData);
        setResponseStatus(status);
        return { success: false, error: errorData };
    } finally {
        setLoading(false);
    }
  };

     
    return {
        loading,
        response,
        responseStatus,
        visitas,
        crearVisita,
        getAllVisitas,
        getVisitasPorRangoDeFechas,
        getReporteVisitasPorRut
    };
};
export default useVisita;