import { useState, useEffect, useCallback } from 'react';
import { useContainer } from '../../providers/containerContext';
import { Pagination } from '../../ui/Pagination';
import { Button } from '../../ui/Button';
import { TextField } from '../../ui/TextField';
import { EmploymentRegimeFormDialog } from './EmploymentRegimeFormDialog';

export function EmploymentRegimesScreen() {
    const { employmentRegimes } = useContainer();
    const [regimes, setRegimes] = useState([]);
    const [page, setPage] = useState(1);
    const [size] = useState(10);
    const [totalPages, setTotalPages] = useState(1);
    const [search, setSearch] = useState('');
    const [isDialogOpen, setIsDialogOpen] = useState(false);
    const [editingRegime, setEditingRegime] = useState(null);
    const [total, setTotal] = useState(0);
    const [error, setError] = useState(null);

    const loadRegimes = useCallback(async (signal) => {
        setError(null);
        try {
            const response = await employmentRegimes.list({ page, size, search }, {signal});
            setRegimes(response.items);
            setTotalPages(response.totalPages);
            setTotal(response.total);
        } catch (error) {
            if (error.name === 'AbortError' || error.name === 'CanceledError') return;
            console.error("Error loading employment regimes", error);
            setError("Error loading employment regimes. Please try again.");
        }
    }, [employmentRegimes, page, size, search]);

    useEffect(() => {
        const controller = new AbortController();

        const delay = setTimeout(() => {
            loadRegimes(controller.signal);
        }, 350);

        return () => {
            clearTimeout(delay);
            controller.abort();
        };
    }, [loadRegimes]);

    const handleToggleActive = async (item) => {
        if (item.active && !window.confirm('Are you sure you want to deactivate this record?')) return;
        setError(null);
        try {
            await employmentRegimes.setActive(item.id, !item.active);

            loadRegimes();
        } catch (error) {
            console.error(error);
            setError("Error updating employment regime status. Please try again.");
        }
    };

    return (
        <div className="p-6">
            <h1 className="text-2xl font-bold mb-4">Employment Regimes</h1>
            {error && (
                <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
                    {error}
                </div>
            )}
            <div className="flex gap-4 mb-4">
                <TextField 
                    placeholder="Search regimes..." 
                    value={search} 
                    onChange={(e) => { setSearch(e.target.value); setPage(1); }} 
                />
                <Button onClick={() => { setEditingRegime(null); setIsDialogOpen(true); }}>
                    New Regime
                </Button>
            </div>

            <table className="w-full text-left border-collapse mb-4">
                <thead>
                    <tr className="border-b">
                        <th className="py-2">Label</th>
                        <th className="py-2">Unit</th>
                        <th className="py-2">Monthly Hours</th>
                        <th className="py-2">Status</th>
                        <th className="py-2">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {regimes.map(regime => (
                        <tr key={regime.id} className="border-b">
                            <td className="py-2">{regime.label}</td>
                            <td className="py-2">{regime.unit}</td>
                            <td className="py-2">{regime.monthlyHours || '-'}</td>
                            <td className="py-2">{regime.active ? 'Active' : 'Inactive'}</td>
                            <td className="py-2 flex gap-2">
                                <Button variant="outline" onClick={() => { setEditingRegime(regime); setIsDialogOpen(true); }}>Edit</Button>
                                <Button variant="outline" onClick={() => handleToggleActive(regime)}>
                                    {regime.active ? "Deactivate" : "Activate"}
                                </Button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <Pagination page={page} totalPages={totalPages} total={total} onChange={setPage} />

            {isDialogOpen && (
                <EmploymentRegimeFormDialog 
                    regime={editingRegime} 
                    onClose={() => setIsDialogOpen(false)} 
                    onSave={() => { setIsDialogOpen(false); loadRegimes(); }} 
                />
            )}
        </div>
    );
}