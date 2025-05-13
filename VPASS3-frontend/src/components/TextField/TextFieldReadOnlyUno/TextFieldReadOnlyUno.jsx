import { TextField } from "@mui/material";

const TextFieldReadOnlyUno = ({ label, value, borderBottomColorAfter = "#175676", borderBottomColorBefore = "#175676", labelColor = "#175676", multiline = false, maxRowsMultiline = 2, width = "100%" }) => {
    return (
      <TextField
        id="standard-read-only-input"
        label={label}
        value={value || ""}  // Asegúrate de que `value` nunca sea `undefined`
        variant="standard"
        multiline = {multiline}
        maxRows={maxRowsMultiline}
        slotProps={
            {htmlInput:{
                readOnly: true
        }}}
        sx={{
          "& .MuiInput-underline:before": {
            borderBottomColor: borderBottomColorBefore,  // Color de la línea cuando no está seleccionada
          },
          "& .MuiInput-underline:after": {
            borderBottomColor: borderBottomColorAfter,  // Color de la línea cuando está seleccionada
          },
          // Estilo para el label
          "& .MuiInputLabel-root": {
            color: labelColor,  // Color del label por defecto
          },
          width: width,
        }}
      />
    );
  }
  export default TextFieldReadOnlyUno;