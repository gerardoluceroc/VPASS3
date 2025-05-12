import { Box, Skeleton, Table, TableBody, TableCell, TableHead, TableRow } from '@mui/material';

const TableSkeleton = ({ columnCount = 6, rowCount = 5 }) => {
  return (
    <Box sx={{ padding: 2 }}>
      <Skeleton variant="text" height={40} width="30%" /> {/* título de la tabla */}

      <Table>
        <TableHead>
          <TableRow>
            {Array.from({ length: columnCount }).map((_, index) => (
              <TableCell key={index}>
                <Skeleton variant="text" width="80%" />
              </TableCell>
            ))}
          </TableRow>
        </TableHead>
        <TableBody>
          {Array.from({ length: rowCount }).map((_, rowIndex) => (
            <TableRow key={rowIndex}>
              {Array.from({ length: columnCount }).map((_, colIndex) => (
                <TableCell key={colIndex}>
                  <Skeleton variant="rectangular" height={30} />
                </TableCell>
              ))}
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </Box>
  );
};

export default TableSkeleton;
