import { Button } from "@mui/material";
import CircularProgress from '@mui/material/CircularProgress';

const ButtonTypeOne = ({
    handleClick = () => {},  // Event handler for onClick event,
    defaultText = "Click here", // Texto que tendrá el boton por defect
    defaultTextColor = "white", // Color del texto por defecto
    loadingText = "loading...", // Texto que tendrá el boton cuando esté en loading
    loadinIconSize = 20, // Tamaño del icono de loading
    loadingIconColor = "white", // Color del icono de loading
    loadingTextColor = "white", // Color del texto de loading
    backgroundColor = "#175676", // Color de fondo por defecto
    backgroundColorHover = "#1F7098", // Color de fondo cuando se hace hover
    letterColorHover = "#B7CBD5", // Color del texto cuando se hace hover
    fontSize = "18px", // Tamaño de la fuente
    fontStyle = "normal", // Estilo de la fuente (normal, italic, oblique)
    fontWeight = 550, // Peso de la fuente (normal, bold, bolder, lighter, etc.)
    lineHeight = "24px", // Altura de la línea
    letterSpacing = "0.15px", // Espaciado entre letras
    textTransform = "none", // Transformación del texto (uppercase, lowercase, capitalize, none)
    loading = false, // Estado de carga del botón
    loadingPosition = "end", // Posición del icono de carga (start, end, center)
    // height = "45px", // Altura del botón
}) => {
  return (
    <Button
        fullWidth
        onClick={handleClick}
        loading={loading}
        loadingPosition={loadingPosition}
        sx={{
            borderRadius: '100px',
            boxShadow: '0px 1px 3px 1px rgba(0, 0, 0, 0.15), 0px 1px 2px 0px rgba(0, 0, 0, 0.30)',
            // height: height,
            color: defaultTextColor,
            background: backgroundColor,
            textAlign: 'center',
            fontSize: fontSize,
            fontStyle: fontStyle,
            fontWeight: fontWeight,
            lineHeight: lineHeight,
            letterSpacing: letterSpacing,
            textTransform: textTransform,
            '&:hover': {  // Aquí se define el estilo del hover
                background: backgroundColorHover,
                color: letterColorHover,
                boxShadow: '0px 1px 3px 1px rgba(0, 0, 0, 0.15), 0px 1px 2px 0px rgba(0, 0, 0, 0.30)',
            },
        }}
        loadingIndicator={

            loadingPosition === "end" ?
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <span style={{ color: loadingTextColor /* tu color personalizado */ }}>
                        {loadingText}
                    </span>
                    <CircularProgress
                        size={loadinIconSize}
                        thickness={5}
                        sx={{ color: loadingIconColor }}
                    />
                </div>
            :

            loadingPosition === "start" ?
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <CircularProgress
                        size={loadinIconSize}
                        thickness={5}
                        sx={{ color: loadingIconColor }}
                    />
                    <span style={{ color: loadingTextColor /* tu color personalizado */ }}>
                        {loadingText}
                    </span>
                </div>

            :

                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center'}}>
                    <CircularProgress
                        size={loadinIconSize}
                        thickness={5}
                        sx={{ color: loadingIconColor }}
                    />
                </div>
        }
    >
        {!loading && defaultText}
    </Button>
  )
}

export default ButtonTypeOne;