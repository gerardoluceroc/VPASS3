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

    const data = rows?.map((espacioComun) => {
        const {name, type, reservations, utilizationLogs} = espacioComun;
        const tipoEspacio = type === idEspacioComunTipoReservable ? "Reservable" : type === idEspacioComunTipoUsable ? "Utilizable" : "Desconocido";
        let columnsRegistros;
        let dataRegistros;

        //En caso de ser un espacio utilizable
        if(type === idEspacioComunTipoUsable){

            columnsRegistros = ["Nombres", "Apellidos", "RUT/Pasaporte", "Hora de inicio", "Tiempo autorizado", "Número de invitados"];

    
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
                    <DatagridResponsive rowsPerPage={5} rowsPerPageOptions={[5, 10]} title={null} searchButton={false} viewColumnsButton={false} columns={columnsRegistros} data={dataRegistros} selectableRows="none"/>
                </AccordionDetails>
            </Accordion>,
            `${tipoEspacio}`.trim(),
        ]
    })
    
    return (
        <Box id="ContainerGestionEspaciosComunesPageComponent">
            <Box id="BotonCrearNuevaListaNegra">
                <ButtonTypeOne
                    defaultText="boton y la ctm"
                    handleClick={()=> console.log("Agregar visitante a lista negra")}
                />
            </Box>
            <Fade in={!(!Array.isArray(rows))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                <DatagridResponsive title="Espacios Comunes" columns={columns} data={data} selectableRows="none" downloadCsvButton={false} /> 
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