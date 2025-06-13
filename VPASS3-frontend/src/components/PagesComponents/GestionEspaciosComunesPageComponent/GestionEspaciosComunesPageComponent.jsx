import { useEffect, useState } from "react";
import useEspaciosComunes from "../../../hooks/useEspaciosComunes/useEspaciosComunes";
import { Accordion, AccordionDetails, AccordionSummary, Box, Fade, Typography } from "@mui/material";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import TableSkeleton from "../../Skeleton/TableSkeleton/TableSkeleton";
import { idEspacioComunTipoReservable, idEspacioComunTipoUsable } from "../../../utils/constantes";
import "./GestionEspaciosComunesPageComponent.css";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { cambiarFormatoHoraFecha, formatoLegibleDesdeHoraString } from "../../../utils/funciones";
import dayjs from "dayjs";
import { useFormik } from "formik";
import ModalReservarEspacioComun from "../../Modal/ModalReservarEspacioComun/ModalReservarEspacioComun";

const GestionEspaciosComunesPageComponent = () => {
    const {loading, getAllEspaciosComunes, espaciosComunes} = useEspaciosComunes();

    useEffect(() => {
      getAllEspaciosComunes();
    }, [])

    // Estado en donde se guardarán los datos de los espacios comunes, es con el objetivo de manipular el arreglo
    const [rows, setRows] = useState();

    useEffect(() => {console.log("📌 - rows => ",rows)}, [rows]);

    // En el momento en que carguen los datos de los espacios comunes, se hace una copia para rows.
    useEffect(() => {
        if (!Array.isArray(espaciosComunes)) return;
        setRows(espaciosComunes);
    }, [espaciosComunes]);

    const columns = [
        "Nombre",
        "Registros",
        "Tipo de espacio",
    ];

    const [openModalReservarEspacioComun, setOpenModalReservarEspacioComun] = useState(false);
    const handleOpenModalReservarEspacioComun = () => setOpenModalReservarEspacioComun(true);  
    const handleCloseModalReservarEspacioComun = () => setOpenModalReservarEspacioComun(false);

    const formik = useFormik({
        initialValues: {
            nombreZona: ''
        },
        // validationSchema: ValidationCrearZona,
        onSubmit: async (values) => {
            // const confirmed = await confirm({
            //     title: "¿Crear zona?",
            //     message: "¿Deseas crear una nueva zona en el establecimiento?"
            // });
        
            // if (confirmed) {
            //     setOpenLoadingRespuesta(true);

            //     // Se envía la información al backend para crear una nueva zona
            //     const {statusCode: statusCodeCrearZona, data: dataZonaAgregada, message: messageCrearZona} = await crearZona(idEstablishment, values.nombreZona);

            //     // Si el servidor responde con el Response dto que tiene configurado
            //     if(statusCodeCrearZona != null && statusCodeCrearZona != undefined){
    
            //       if (statusCodeCrearZona === 200 || statusCodeCrearZona === 201) {
            //         setOperacionExitosa(true);
            //         setMessageLoadingRespuesta(messageCrearZona);
            //         setRows(prevRows => [...prevRows, dataZonaAgregada]); // Agrega la nueva zona a las filas de la tabla
            //         formik.resetForm(); // Resetea el formulario después de crear la zona
            //       }
            //       else if (statusCodeCrearZona === 500) {
            //           //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
            //           setOperacionExitosa(false);
            //           setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
            //       }
            //       else{
            //           //En caso de cualquier otro error, se muestra el mensaje de error del backend
            //           setOperacionExitosa(false);
            //           setMessageLoadingRespuesta(messageCrearZona);
            //       }
            //     }
            //     else{
            //       //Esto es para los casos que el servidor no responda el ResponseDto tipico
            //       setOperacionExitosa(false);
            //       setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
            //     }

            // } 
        }
    });
    useEffect(() => {console.log("📌 - formik values => ",formik.values)}, [formik.values]);











    const data = rows?.map((espacioComun) => {
        const {name, type, reservations, utilizationLogs} = espacioComun;
        const tipoEspacio = type === idEspacioComunTipoReservable ? "Reservable" : type === idEspacioComunTipoUsable ? "Utilizable" : "Desconocido";
        let columnsRegistros;
        let dataRegistros;

        //En caso de ser un espacio utilizable
        if(type === idEspacioComunTipoUsable){

            columnsRegistros = ["Nombres", "Apellidos", "RUT/Pasaporte", "Fecha de inicio", "Tiempo autorizado", "Número de invitados"];

            const registrosUsoOrdenadosPorFecha = [...utilizationLogs].sort((a, b) =>
                dayjs(b.startTime).valueOf() - dayjs(a.startTime).valueOf()
            );

            dataRegistros = registrosUsoOrdenadosPorFecha.map((utilizationLog) => {
                const {guestsNumber , person, startTime, usageTime} = utilizationLog || {};
                const {names: namePersonaRegistradaEspacioUtilizable = "", lastNames: lastNamePersonaRegistradaEspacioUtilizable  = "", identificationNumber: rutPersonaRegistradaEspacioUtilizable = ""} = person || {};

                return [
                    `${namePersonaRegistradaEspacioUtilizable}`.trim(),
                    `${lastNamePersonaRegistradaEspacioUtilizable}`.trim(),
                    `${rutPersonaRegistradaEspacioUtilizable}`.trim(),
                    `${cambiarFormatoHoraFecha(startTime)}`,
                    `${formatoLegibleDesdeHoraString(usageTime)}`.trim(),
                    guestsNumber !== null ? `${guestsNumber}`.trim() : 0
                ]
            })
        }

        //En caso de ser un espacio reservable
        if(type === idEspacioComunTipoReservable){

            columnsRegistros = ["Nombres", "Apellidos", "RUT/Pasaporte", "Fecha de inicio", "Tiempo autorizado", "Número de invitados"];

            const registrosReservasOrdenadosPorFecha = [...reservations].sort((a, b) =>
                dayjs(b.reservationStart).valueOf() - dayjs(a.reservationStart).valueOf()
            );

            dataRegistros = registrosReservasOrdenadosPorFecha.map((reservation) => {
                const {reservationTime, reservationStart, reservedBy, guests} = reservation || {};
                const {names: namesReservado = "", lastNames: lastNamesReservado = "", identificationNumber: rutReservado = ""} = reservedBy || {};

                return [
                    `${namesReservado}`.trim(),
                    `${lastNamesReservado}`.trim(),
                    `${rutReservado}`.trim(),
                    `${cambiarFormatoHoraFecha(reservationStart)}`,
                    `${formatoLegibleDesdeHoraString(reservationTime)}`,
                    `${guests}`.trim() || 0
                ]
            })
        }

        return[
            `${name}`.trim(),
            <Accordion id="AccordionRegistroEspaciosComunes">
                <AccordionSummary
                    expandIcon={<ExpandMoreIcon />}
                    aria-controls="panel1-content"
                    id="panel1-header"
                >
                    {type === idEspacioComunTipoUsable ?
                        <Typography variant="h6">{`Registros de uso`}</Typography> 

                    :

                    type === idEspacioComunTipoReservable ?
                        <Typography variant="h6">{`Registros de Reservas`}</Typography>

                    : 

                    null
                    }
                </AccordionSummary>
                <AccordionDetails>
                    <DatagridResponsive rowsPerPage={5} rowsPerPageOptions={[5, 10]} title={null} searchButton={true} viewColumnsButton={false} columns={columnsRegistros} data={dataRegistros} selectableRows="none"/>
                </AccordionDetails>
            </Accordion>,
            `${tipoEspacio}`.trim(),
        ]
    })
    
    return (
        <Box id="ContainerGestionEspaciosComunesPageComponent">
            <Box id="BotonCrearNuevaReservaEspacioComun">
                <ButtonTypeOne
                    defaultText="Reservar espacio común"
                    handleClick={handleOpenModalReservarEspacioComun}
                />
            </Box>
            <Fade in={!(!Array.isArray(rows))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                <DatagridResponsive title="Espacios Comunes" columns={columns} data={data} selectableRows="none" downloadCsvButton={false} /> 
                <ModalReservarEspacioComun
                    open={openModalReservarEspacioComun}
                    onClose={handleCloseModalReservarEspacioComun}
                    setEspaciosComunes={setRows}
                    espaciosComunes={rows}
                />
                </div>
            </Fade>

            <Fade in={!Array.isArray(rows)} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                    <TableSkeleton columnCount={3} rowCount={7} />
                </div>
            </Fade>
        </Box>
    )
}

export default GestionEspaciosComunesPageComponent;