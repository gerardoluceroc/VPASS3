import "./ModalVerDetallesEncomienda.css";
import { Box, IconButton, Modal, TextField } from "@mui/material";
import CloseIcon from '@mui/icons-material/Close';
import TextFieldReadOnlyUno from "../../TextField/TextFieldReadOnlyUno/TextFieldReadOnlyUno";
import { cambiarFormatoHoraFecha } from "../../../utils/funciones";

const ModalVerDetallesEncomienda = ({ open, onClose, encomiendaSeleccionada, departamentoSeleccionado }) => {

    const {
        code: codigoEncomienda, 
        receivedAt: fechaLlegada, 
        deliveredAt: fechaEntrega,
        ownership: propietario,
        recipient: destinatario,
        receiver: personaQueRetira,
    } = encomiendaSeleccionada;

    const {zoneName: zonaDestino, name: nombreDepartamento} = departamentoSeleccionado || {};

    const {names: nombrePersonaQueRetira, lastNames: apellidoPersonaQueRetira, identificationNumber: rutPersonaQueRetira} = personaQueRetira || {};


    return (
        <Modal open={open} onClose={onClose}>
        <Box id="ContainerModalVerDetallesEncomienda">
            <Box id="HeaderModalVerDetallesEncomienda">
                <IconButton
                aria-label="close"
                onClick={onClose}
                sx={{
                    position: 'absolute',
                    right: 8,
                    top: 8,
                    color: "black",
                }}
                >
                <CloseIcon />
                </IconButton>
            </Box>
            <Box id="CuerpoModalVerDetallesEncomienda">
                <Box className="DosItemsCuerpoModalVerDetallesEncomienda">
                    <TextFieldReadOnlyUno
                        label={"Destinatario"}
                        value={`${destinatario}` || "Sin datos"}
                    />

                    <TextFieldReadOnlyUno
                        label={"Lugar de destino"}
                        value={`${zonaDestino} - ${nombreDepartamento}` || "Sin datos"}
                    />
                </Box>

                <Box className="DosItemsCuerpoModalVerDetallesEncomienda">
                    <TextFieldReadOnlyUno
                        label={"Código de encomienda"}
                        value={codigoEncomienda === null ? "Sin datos" : `${codigoEncomienda}`}
                    />

                    <TextFieldReadOnlyUno
                        label={"Fecha de llegada"}
                        value={`${cambiarFormatoHoraFecha(fechaLlegada)}` || "Sin datos"}
                    />
                </Box>

                <Box className="DosItemsCuerpoModalVerDetallesEncomienda">
                    <TextFieldReadOnlyUno
                        label={"Estado"}
                        value={fechaEntrega === null ? "Pendiente" : "Retirado"}
                    />

                    {fechaEntrega !== null && 

                        <TextFieldReadOnlyUno
                            label={"Fecha de entrega"}
                            value={`${cambiarFormatoHoraFecha(fechaEntrega)}` || "Sin datos"}
                        />              
                    }
                </Box>

                {
                    personaQueRetira !== null && (
                        <TextFieldReadOnlyUno
                            label={"Persona que retira"}
                            value={`${nombrePersonaQueRetira} ${apellidoPersonaQueRetira} - Rut: ${rutPersonaQueRetira}` || "Sin datos"}
                        />
                    )
                }
            </Box>
        </Box>
        </Modal>
    );
};

export default ModalVerDetallesEncomienda;