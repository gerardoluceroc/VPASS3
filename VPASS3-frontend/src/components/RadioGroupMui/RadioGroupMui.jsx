import React from 'react';
import {
  FormControl,
  FormLabel,
  RadioGroup,
  FormControlLabel,
  Radio,
  Box,
  FormHelperText
} from '@mui/material';

/**
 * Componente RadioGroup de MaterialUI personalizado para ser reutilizado
 * 
 * Props:
 * - name: Nombre del campo (opcional)
 * - label: Etiqueta del grupo de radios (opcional)
 * - handleChange: Función que maneja el cambio de selección (opcional)
 * - listadoElementos: Array de objetos con las opciones (requerido)
 * - keyListadoElementos: Atributo que sirve como key única (opcional, default: "id")
 * - atributoValue: Atributo del objeto que se usará como valor (opcional, default: "value")
 * - elementoSeleccionado: Valor seleccionado actualmente
 * - mostrarElemento: Función que determina cómo mostrar cada opción (opcional)
 * - row: Si los radios deben mostrarse en fila (opcional, default: false)
 * - disabled: Si todo el grupo está deshabilitado (opcional)
 * - disabledOptionCondition: Función para deshabilitar opciones específicas (opcional)
 * - helperText: Texto de ayuda (opcional)
 * - error: Si muestra error (opcional)
 * - sx: Estilos personalizados (opcional)
 */
const RadioGroupMui = ({
  name = "",
  label = "Seleccione una opción",
  handleChange = () => {},
  listadoElementos = [],
  keyListadoElementos = "id",
  atributoValue = "value",
  elementoSeleccionado,
  mostrarElemento = (opcion) => opcion.label || opcion[atributoValue],
  row = false,
  disabled = false,
  disabledOptionCondition = (opcion) => false,
  helperText = "",
  error = false,
  sx = {}
}) => {
  return (
    <Box sx={sx}>
      <FormControl component="fieldset" error={error} disabled={disabled}>
        <FormLabel sx={{display: "flex", justifyContent: "center", width: "100%"}} component="legend">{label}</FormLabel>
        <RadioGroup
          name={name}
          // value={elementoSeleccionado || ""}
          value={elementoSeleccionado !== undefined && elementoSeleccionado !== null ? elementoSeleccionado : ""}
          onChange={handleChange}
          row={row}
          sx={{display: "flex", justifyContent: "center", width: "100%"}}
        >
          {listadoElementos.map((item) => (
            <FormControlLabel
              key={item[keyListadoElementos]}
              value={item[atributoValue]}
              control={<Radio />}
              label={mostrarElemento(item)}
              disabled={disabled || disabledOptionCondition(item)}
            />
          ))}
        </RadioGroup>
        {helperText && <FormHelperText>{helperText}</FormHelperText>}
      </FormControl>
    </Box>
  );
};

export default RadioGroupMui;