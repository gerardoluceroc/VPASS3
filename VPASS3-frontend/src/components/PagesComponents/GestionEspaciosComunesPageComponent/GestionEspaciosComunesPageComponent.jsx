import { useEffect, useState } from "react";
import useEspaciosComunes from "../../../hooks/useEspaciosComunes/useEspaciosComunes";
import { Accordion, AccordionDetails, AccordionSummary, Box, Button, Fade, Typography } from "@mui/material";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import TableSkeleton from "../../Skeleton/TableSkeleton/TableSkeleton";
import { CommonAreaMode } from "../../../utils/constantes";
import "./GestionEspaciosComunesPageComponent.css";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import { cambiarFormatoHoraFecha, filtrarReservasExclusivasActivas, filtrarReservasExclusivasFinalizadas, filtrarUsosCompartidosActivos, filtrarUsosCompartidosFinalizados, formatoLegibleDesdeHoraString } from "../../../utils/funciones";
import dayjs from "dayjs";
import ModalReservarEspacioComun from "../../Modal/ModalReservarEspacioComun/ModalReservarEspacioComun";

const GestionEspaciosComunesPageComponent = () => {
    // Hook con los espacios y su informacion detallada, incluyendo reservas, nombre, etc.
    const { getAllEspaciosComunes, espaciosComunes} = useEspaciosComunes();

    // Se obtienen los espacios comunes desde el hook
    useEffect(() => {
      getAllEspaciosComunes();
    }, [])

    // Estado en donde se guardarán los datos de los espacios comunes, es con el objetivo de manipular el arreglo
    const [rowsOriginales, setRowsOriginales] = useState();
    
    const [rowsModificables, setRowsModificables] = useState();

    // En el momento en que carguen los datos de los espacios comunes, se hace una copia para rows.
    useEffect(() => {
        if (!Array.isArray(espaciosComunes)) return;
        setRowsOriginales(espaciosComunes);
        setRowsModificables(espaciosComunes);
    }, [espaciosComunes]);

    // Estados y funciones para abrir y cerrar el modal de crear una nueva reserva
    const [openModalReservarEspacioComun, setOpenModalReservarEspacioComun] = useState(false);
    const handleOpenModalReservarEspacioComun = () => setOpenModalReservarEspacioComun(true);  
    const handleCloseModalReservarEspacioComun = () => setOpenModalReservarEspacioComun(false);

    // Variables y funciones para manejar el filtrado de las reservas exclusivas
    const idOpcionTodasBotonesTablaRegistroReservasExclusiva = 1;
    const idOpcionActivasBotonesTablaRegistroReservasExclusivas = 2;
    const idOpcionFinalizadasBotonesTablaRegistroReservasExclusivas = 3;
    const handleClickBotonOpcionesTablaRegistroReservaExclusiva = (espacioComun, idOpcion) => {
        if (!rowsOriginales) return;

        // Se trabaja sobre la versión original (sin filtros)
        const espacioOriginal = rowsOriginales.find(row => row.id === espacioComun.id);
        if (!espacioOriginal) return;

        let usosCompartidosFiltrados = [];

        if (idOpcion === idOpcionTodasBotonesTablaRegistroReservasExclusiva) {
            usosCompartidosFiltrados = espacioOriginal.reservations;
        } else if (idOpcion === idOpcionActivasBotonesTablaRegistroReservasExclusivas) {
            usosCompartidosFiltrados = filtrarReservasExclusivasActivas(espacioOriginal.reservations);
        } else if (idOpcion === idOpcionFinalizadasBotonesTablaRegistroReservasExclusivas) {
            usosCompartidosFiltrados = filtrarReservasExclusivasFinalizadas(espacioOriginal.reservations);
        }

        const espacioActualizado = {
            ...espacioOriginal,
            reservations: usosCompartidosFiltrados,
        };

        // Se aplica el cambio solo a ese espacio en la lista modificable
        const nuevaListaModificable = rowsOriginales.map(row =>
            row.id === espacioComun.id ? espacioActualizado : row
        );
        setRowsModificables(nuevaListaModificable);
    };


    const idOpcionTodosBotonesTablaRegistroUsoCompartido = 1;
    const idOpcionActivosBotonesTablaRegistroUsoCompartido = 2;
    const idOpcionFinalizadosBotonesTablaRegistroUsoCompartido = 3;
    const handleClickBotonOpcionesTablaRegistroUsoCompartido = (espacioComun, idOpcion) => {
        if (!rowsOriginales) return;

        // Se trabaja sobre la versión original (sin filtros)
        const espacioOriginal = rowsOriginales.find(row => row.id === espacioComun.id);
        if (!espacioOriginal) return;

        let usosCompartidosFiltrados = [];

        if (idOpcion === idOpcionTodosBotonesTablaRegistroUsoCompartido) {
            usosCompartidosFiltrados = espacioOriginal.usages;
        } else if (idOpcion === idOpcionActivosBotonesTablaRegistroUsoCompartido) {
            usosCompartidosFiltrados = filtrarUsosCompartidosActivos(espacioOriginal.usages);
        } else if (idOpcion === idOpcionFinalizadosBotonesTablaRegistroUsoCompartido) {
            usosCompartidosFiltrados = filtrarUsosCompartidosFinalizados(espacioOriginal.usages);
        }

        const espacioActualizado = {
            ...espacioOriginal,
            usages: usosCompartidosFiltrados,
        };

        // Se aplica el cambio solo a ese espacio en la lista modificable
        const nuevaListaModificable = rowsOriginales.map(row =>
            row.id === espacioComun.id ? espacioActualizado : row
        );
        setRowsModificables(nuevaListaModificable);
    };


    const columns = [
        "Nombre",
        "Registros",
    ];

    const data = rowsModificables?.map((espacioComun) => {
        const {name, mode, reservations, usages} = espacioComun;
        const registrosUsoOrdenadosPorFecha = [...usages].sort((a, b) =>
            dayjs(b.startTime).valueOf() - dayjs(a.startTime).valueOf()
        );
        const columnsRegistrosUso = ["Nombres", "Apellidos", "RUT/Pasaporte", "Fecha de inicio", "Tiempo autorizado", "Número de invitados"];
        const dataRegistrosUso = registrosUsoOrdenadosPorFecha.map((usage) => {
            const {guestsNumber , person, startTime, usageTime} = usage || {};
            const {names: namePersonaRegistradaEspacioUtilizable = "", lastNames: lastNamePersonaRegistradaEspacioUtilizable  = "", identificationNumber: rutPersonaRegistradaEspacioUtilizable = ""} = person || {};

            return [
                `${namePersonaRegistradaEspacioUtilizable}`.trim(),
                `${lastNamePersonaRegistradaEspacioUtilizable}`.trim(),
                `${rutPersonaRegistradaEspacioUtilizable}`.trim(),
                `${cambiarFormatoHoraFecha(startTime)}`,
                `${formatoLegibleDesdeHoraString(usageTime)}`.trim() || "No indicado",
                guestsNumber !== null ? `${guestsNumber}`.trim() : 0
            ]
        });

        const registrosReservasOrdenadosPorFecha = [...reservations].sort((a, b) =>
            dayjs(b.reservationStart).valueOf() - dayjs(a.reservationStart).valueOf()
        );
        const columnsRegistrosReservas = ["Nombres", "Apellidos", "RUT/Pasaporte", "Fecha de inicio", "Tiempo autorizado", "Número de invitados"];
        const dataRegistrosReservas = registrosReservasOrdenadosPorFecha.map((reservation) => {
            const {reservationTime, reservationStart, reservedBy, guestsNumber} = reservation || {};
            const {names: namesReservado = "", lastNames: lastNamesReservado = "", identificationNumber: rutReservado = ""} = reservedBy || {};

            return [
                `${namesReservado}`.trim(),
                `${lastNamesReservado}`.trim(),
                `${rutReservado}`.trim(),
                `${cambiarFormatoHoraFecha(reservationStart)}`,
                `${formatoLegibleDesdeHoraString(reservationTime) || "No indicado"}`,
                guestsNumber !== null ? `${guestsNumber}`.trim() : 0
            ]
        })

        return[
            `${name}`.trim(),
            <Box id="RegistrosUsoReservaGestionEspaciosComunes">

                {/* Si el espacio es de tipo usable se muestran sus registros de uso */}
                {((mode & CommonAreaMode.Usable) !== 0) &&
                    <Accordion id="AccordionRegistroEspaciosComunes">
                        <AccordionSummary
                            expandIcon={<ExpandMoreIcon />}
                            aria-controls="panel1-content"
                            id="panel1-header"
                        >

                            <Typography variant="h6">{`Registros de uso`}</Typography> 
                        </AccordionSummary>

                        <AccordionDetails>
                            <Box className="BoxBotonesFiltrarRegistroEspaciosComunes">
                                <ButtonTypeOne handleClick={()=>{handleClickBotonOpcionesTablaRegistroUsoCompartido(espacioComun, idOpcionTodasBotonesTablaRegistroReservasExclusiva)}} fontSize="15px" width="30%" defaultText="Todos"/>
                                <ButtonTypeOne handleClick={()=>{handleClickBotonOpcionesTablaRegistroUsoCompartido(espacioComun, idOpcionActivasBotonesTablaRegistroReservasExclusivas)}} fontSize="15px" width="30%" defaultText="Activos"/>
                                <ButtonTypeOne handleClick={()=>{handleClickBotonOpcionesTablaRegistroUsoCompartido(espacioComun, idOpcionFinalizadasBotonesTablaRegistroReservasExclusivas)}} fontSize="15px" width="30%" defaultText="Finalizados"/>
                            </Box>
                            <DatagridResponsive rowsPerPage={5} rowsPerPageOptions={[5, 10]} title={null} searchButton={false} viewColumnsButton={false} columns={columnsRegistrosUso} data={dataRegistrosUso} selectableRows="none"/>
                        </AccordionDetails>
                    </Accordion> 
                }
                
                {/* Si el espacio es reservable se muestran sus registros */}
                {((mode & CommonAreaMode.Reservable) !== 0) &&
                    <Accordion id="AccordionRegistroEspaciosComunes">
                        <AccordionSummary
                            expandIcon={<ExpandMoreIcon />}
                            aria-controls="panel1-content"
                            id="panel1-header"
                        >
                            <Typography variant="h6">{`Registros de Reservas`}</Typography>

                        </AccordionSummary>
                        <AccordionDetails>
                            <Box className="BoxBotonesFiltrarRegistroEspaciosComunes">
                                <ButtonTypeOne handleClick={()=>{handleClickBotonOpcionesTablaRegistroReservaExclusiva(espacioComun, idOpcionTodasBotonesTablaRegistroReservasExclusiva)}} fontSize="15px" width="30%" defaultText="Todas"/>
                                <ButtonTypeOne handleClick={()=>{handleClickBotonOpcionesTablaRegistroReservaExclusiva(espacioComun, idOpcionActivasBotonesTablaRegistroReservasExclusivas)}} fontSize="15px" width="30%" defaultText="Activas"/>
                                <ButtonTypeOne handleClick={()=>{handleClickBotonOpcionesTablaRegistroReservaExclusiva(espacioComun, idOpcionFinalizadasBotonesTablaRegistroReservasExclusivas)}} fontSize="15px" width="30%" defaultText="Finalizadas"/>
                            </Box>
                            <DatagridResponsive rowsPerPage={5} rowsPerPageOptions={[5, 10]} title={null} searchButton={false} viewColumnsButton={false} columns={columnsRegistrosReservas} data={dataRegistrosReservas} selectableRows="none"/>
                        </AccordionDetails>
                    </Accordion>       
                }
            </Box> 
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
            <Fade in={!(!Array.isArray(rowsOriginales))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                <DatagridResponsive title="Espacios Comunes" columns={columns} data={data} selectableRows="none" downloadCsvButton={false} /> 
                <ModalReservarEspacioComun
                    open={openModalReservarEspacioComun}
                    onClose={handleCloseModalReservarEspacioComun}
                    setEspaciosComunesOriginales={setRowsOriginales}
                    setEspaciosComunesModificables={setRowsModificables}
                    espaciosComunes={rowsOriginales}
                />
                </div>
            </Fade>

            <Fade in={!Array.isArray(rowsOriginales)} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                    <TableSkeleton columnCount={3} rowCount={7} />
                </div>
            </Fade>
        </Box>
    )
}

export default GestionEspaciosComunesPageComponent;