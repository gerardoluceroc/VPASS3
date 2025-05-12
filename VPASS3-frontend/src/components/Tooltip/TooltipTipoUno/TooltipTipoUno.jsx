import { Tooltip } from "@mui/material";
import "./TooltipTipoUno.css";

const TooltipTipoUno = ({ children, titulo, ubicacion}) =>{
    return (
        <Tooltip id="TooltipTipoUno" title={titulo} placement={ubicacion} componentsProps={{
            tooltip: {
              sx: {
                bgcolor: '#1F7098',
                borderRadius: "4px",
                boxShadow: "0px 1px 3px 1px rgba(0, 0, 0, 0.15), 0px 1px 2px 0px rgba(0, 0, 0, 0.30)",
              }
            },
            }}>
            {
                children
            }
        </Tooltip>
    );
}
export default TooltipTipoUno;