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
    loadingPosition = "center", // Posición del icono de carga (start, end, center)
    width = "100%",
    // height = "45px", // Altura del botón
}) => {
   // 🔧 Calcula altura mínima basada en el fontSize
  const computeMinHeight = (fontSize) => {
    const size = parseInt(fontSize.replace('px', ''), 10);
    return `${size * 3}px`; // puedes ajustar el multiplicador según estilo deseado
  };

  const minHeight = computeMinHeight(fontSize);

  //  Estilo común para loading text e ícono
  const loadingContentStyle = {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
    fontSize,
    fontStyle,
    fontWeight,
    lineHeight,
    letterSpacing,
    textTransform,
    color: loadingTextColor,
    gap: "10px",
  };

  //  loadingIndicator según posición
  const loadingIndicator = (
    <div style={loadingContentStyle}>
      {(loadingPosition === 'start' || loadingPosition === 'center') && (
        <CircularProgress
          size={loadinIconSize}
          thickness={5}
          sx={{ color: loadingIconColor, marginRight: loadingPosition === 'start' ? '8px' : '0' }}
        />
      )}
      <span>{loadingText}</span>
      {loadingPosition === 'end' && (
        <CircularProgress
          size={loadinIconSize}
          thickness={5}
          sx={{ color: loadingIconColor, marginLeft: '8px' }}
        />
      )}
    </div>
  );

  return (
    <Button
      onClick={handleClick}
      loading={loading}
      loadingPosition={loadingPosition}
      sx={{
        width,
        minHeight,
        borderRadius: '100px',
        boxShadow:
          '0px 1px 3px 1px rgba(0, 0, 0, 0.15), 0px 1px 2px 0px rgba(0, 0, 0, 0.30)',
        color: defaultTextColor,
        background: backgroundColor,
        textAlign: 'center',
        fontSize,
        fontStyle,
        fontWeight,
        lineHeight,
        letterSpacing,
        textTransform,
        '&:hover': {
          background: backgroundColorHover,
          color: letterColorHover,
          boxShadow:
            '0px 1px 3px 1px rgba(0, 0, 0, 0.15), 0px 1px 2px 0px rgba(0, 0, 0, 0.30)',
        },
      }}
      loadingIndicator={loadingIndicator}
    >
      {!loading && defaultText}
    </Button>
  );
};

export default ButtonTypeOne;