import { IconButton } from "@mui/material"
import DeleteIcon from '@mui/icons-material/Delete';
import TooltipTipoUno from "../Tooltip/TooltipTipoUno/TooltipTipoUno";

export const IconoBorrar = ({
  handleClick = () => {},
  backgroundColorNormal = "transparent",
  backgroundColorHover = "#175676",
  IconColorNormal = "#175676",
  IconColorHover = "white",
  size = "30px",
  tituloToolTip = "Eliminar",
  ubicacionToolTip = "right"
}) => {
  return (
    <TooltipTipoUno titulo={tituloToolTip} ubicacion={ubicacionToolTip}>
      <IconButton
        onClick={handleClick}
        sx={{
          backgroundColor: backgroundColorNormal,
          "&:hover": {
            backgroundColor: backgroundColorHover,
            "& .MuiSvgIcon-root": {
              color: IconColorHover,
            },
          },
        }}
      >
        <DeleteIcon
          sx={{
            fontSize: size,
            color: IconColorNormal,
          }}
        />
      </IconButton>
    </TooltipTipoUno>
  );
};
